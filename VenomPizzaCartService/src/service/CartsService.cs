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

    public async Task<Cart> GetCartById(int id)
    {
        var cartFromCache = await _cacheProvider.GetAsync<Cart>(id);
        if (cartFromCache != null)
            return cartFromCache;
        var foundedCart= await _cartRepository.GetCartById(id);
        if (foundedCart == null)
            return new Cart() { Id = id };
        foundedCart.TotalPrice = CalculateCartPrice(foundedCart);
        await _cacheProvider.SetAsync(id, foundedCart,cartExpiration);
        return foundedCart;
    }

    public async Task<decimal> GetCartPrice(int id)
    {
        var cartFromCache = await _cacheProvider.GetAsync<Cart>(id);
        if (cartFromCache != null)
            return cartFromCache.TotalPrice;

        var cart = await GetCartById(id);
        if (cart == null)
            return 0;
        var sum=CalculateCartPrice(cart);

        _logger.LogInformation($"Цена товаров из корзины {id}: {sum}");

        return sum;
    }

    public async Task<OrderRequestDto> CreateOrder(int cartId,string address,DateTime? byTheTime)
    {
        _logger.LogInformation($"Начата сборка заказа корзины {cartId}");
        var cart=await _cartRepository.GetCartById(cartId);
        if (cart==null || cart.Products.Count == 0)
            throw new BadHttpRequestException("Нельзя создать заказ с пустой корзиной");
        List<OrderProductDto> orderProducts = cart.Products.Select(x =>
        {
            var cache = _cloudStorageProvider.GetProductCacheById(x.ProductId);
            if (cache == null)
                throw new NullReferenceException($"Не найден продукт с Id {x.ProductId} в кэше");
            var price = cache.Prices.FirstOrDefault(p => p.PriceId == x.PriceId);
            if (price == null)
                throw new NullReferenceException($"Не найдена цена {x.PriceId} продукта с Id {x.ProductId} в кэше");
            return new OrderProductDto(x.ProductId, cache.Title, price.Size, cache.ImageUrl, x.Quantity, price.Price);
        }).ToList();
        OrderRequestDto orderRequestDto = new OrderRequestDto(cartId,orderProducts,CalculateCartPrice(cart),address,byTheTime);
        _logger.LogInformation($"Собран заказ {orderRequestDto.Id} на сумму {orderRequestDto.Price}");
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

    private decimal CalculateCartPrice(Cart cart)
    {
        _logger.LogInformation($"Подсчет цены товаров из корзины {cart.Id}");

        var productsId = cart.Products.Select(p => p.ProductId).Distinct().ToList();
        var products = _cloudStorageProvider.GetProductsCacheById(productsId).ToDictionary(x => x.Id);
        var sum = cart.Products.Sum(product =>
        {
            var price = products[product.ProductId].Prices[product.PriceId].Price;
            return price * product.Quantity;
        });
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