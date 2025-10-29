using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.etc;
using VenomPizzaCartService.src.Kafka;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.providers;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public class CartsService:ICartsService
{
    private readonly TimeSpan cartExpiration = TimeSpan.FromMinutes(15);

    private readonly ICartsRepository _cartRepository;
    private readonly ICloudStorageProvider _cloudStorageProvider;
    private readonly ILogger<CartsService> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ICacheProvider _cacheProvider;
    public CartsService(ICartsRepository repository,ICloudStorageProvider cloudStorageProvider, ILogger<CartsService> logger, 
        IProducer<string,string> producer, IOptions<KafkaSettings> kafkaSettings,ICacheProvider cacheProvider)
    {
        _cartRepository = repository;
        _cloudStorageProvider = cloudStorageProvider;
        _logger = logger;
        _producer = producer;
        _kafkaSettings = kafkaSettings.Value;
        _cacheProvider = cacheProvider;
    }

    public async Task<CartDto> GetCartById(int id)
    {
        var cartFromCache = await _cacheProvider.GetAsync<CartDto>(id);
        if (cartFromCache != null)
            return cartFromCache;
        var foundedCart= await _cartRepository.GetCartById(id);
        if (foundedCart == null)
            return new CartDto(id);
        var cartDto = new CartDto(foundedCart);
        AddProductInfoInCart(cartDto);
        cartDto.TotalPrice = CalculateCartPrice(cartDto);
        await _cacheProvider.SetAsync(id, cartDto,cartExpiration);
        return cartDto;
    }
    
    public async Task<OrderRequestDto> CreateOrder(int cartId,string address,DateTime? byTheTime)
    {
        _logger.LogInformation($"Начата сборка заказа корзины {cartId}");
        var foundedCart=await _cartRepository.GetCartById(cartId);
        if (foundedCart==null || foundedCart.Products.Count == 0)
            throw new BadHttpRequestException("Нельзя создать заказ с пустой корзиной");
        List<OrderProductDto> orderProducts = foundedCart.Products.Select(x =>
        {
            var cache = _cloudStorageProvider.GetProductCacheById(x.ProductId);
            if (cache == null)
                throw new NullReferenceException($"Не найден продукт с Id {x.ProductId} в кэше");
            if (!cache.IsAvailable)
                throw new BadHttpRequestException($"Продукт {cache.Title} с Id {cache.Id} не доступен для заказа на данный момент");
            var price = cache.Prices.FirstOrDefault(p => p.PriceId == x.PriceId);
            if (price == null)
                throw new NullReferenceException($"Не найдена цена {x.PriceId} продукта с Id {x.ProductId} в кэше");
            return new OrderProductDto(x.ProductId, cache.Title, price.Size, cache.ImageUrl, x.Quantity, price.Price);
        }).ToList();
        var cartDto = new CartDto(foundedCart);
        AddProductInfoInCart(cartDto);
        OrderRequestDto orderRequestDto = new OrderRequestDto
            (cartId,orderProducts,CalculateCartPrice(cartDto),address,byTheTime);
        _logger.LogInformation($"Создание заказа {orderRequestDto.Id} на сумму {orderRequestDto.Price}");
        await SendInOrderRequestCreatedTopic(orderRequestDto);
        return orderRequestDto;
    }

    public async Task<CartProduct> AddProductToCart(int cartId, int productId, int priceId, int quantity)
    {
        var cart = await _cartRepository.GetCartById(cartId);
        if(cart==null)
            cart=await _cartRepository.CreateCart(cartId);
        else
            await _cacheProvider.RemoveAsync<Cart>(cartId);

        var existingProduct = await _cartRepository.GetProductById(cartId, productId,priceId);
        if (existingProduct != null)
            return await _cartRepository.UpdateProductQuantity(cartId,productId,priceId, quantity);
        else
            return await _cartRepository.AddProduct(cartId, productId,priceId, quantity);
    }

    public async Task<CartProduct> UpdateProductQuantity(int cartId, int productId,int priceId, int quantity)
    {
        var product=await _cartRepository.UpdateProductQuantity(cartId, productId, priceId, quantity);
        await _cacheProvider.RemoveAsync<Cart>(cartId);
        return product;
    }

    public async Task DeleteProductInCart(int cartId, int productId, int priceId)
    {
        await _cartRepository.DeleteProductInCart(cartId, productId, priceId);
        await _cacheProvider.RemoveAsync<Cart>(cartId);
    }

    public async Task AddProductInfo(ProductShortInfoDto product)
    {
        await _cloudStorageProvider.AddProductInfo(product);
    }

    public async Task UpdateProductInfo(ProductShortInfoDto product)
    {
        await _cloudStorageProvider.UpdateProductInfo(product);
    }

    public async Task DeleteProductInfo(ProductShortInfoDto product)
    {
        await _cloudStorageProvider.DeleteProductInfo(product.Id);
    }

    private void AddProductInfoInCart(CartDto cartDto)
    {
        foreach (var product in cartDto.Products)
        {
            var foundedShortInfo=_cloudStorageProvider.GetProductCacheById(product.ProductId);
            if (foundedShortInfo == null)
                throw new KeyNotFoundException($"Не найдена информация о продукте {product.ProductId} в корзине {cartDto.Id}");
            var productPrice=foundedShortInfo.Prices.FirstOrDefault(p => p.PriceId == product.PriceId);
            if(productPrice==null)
                throw new ($"Не найдена цена {product.PriceId} продукта {product.ProductId} в корзине {cartDto.Id}");
            product.AddInfo(foundedShortInfo.Title,productPrice.Price,foundedShortInfo.ImageUrl,foundedShortInfo.IsAvailable);
        }
    }

    private decimal CalculateCartPrice(CartDto cart)
    {
        _logger.LogInformation($"Подсчет цены товаров из корзины {cart.Id}");
        
        var sum = cart.Products.Sum(product => product.Price * product.Quantity);
        return sum;
    }

    private async Task SendInOrderRequestCreatedTopic(OrderRequestDto dto)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = dto.UserId.ToString(),
            Value = JsonSerializer.Serialize(dto)
        };
        await _producer.ProduceAsync(_kafkaSettings.Topics.OrderRequestCreated, kafkaMessage);
        _logger.LogInformation($"Отправлено {kafkaMessage.Value} в {_kafkaSettings.Topics.OrderRequestCreated}");
    }
}