using System.Collections.Concurrent;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.etc;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public class CartsService:ICartsService
{
    private readonly ICartsRepository _cartRepository;
    private readonly ICacheManager _cacheManager;
    public CartsService(ICartsRepository repository,ICacheManager cacheManager)
    {
        _cartRepository = repository;
        _cacheManager = cacheManager;
    }

    public async Task<Cart> GetCartById(int id)
    {
        var foundedCart= await _cartRepository.GetCartById(id);
        if (foundedCart == null)
            return new Cart() { Id = id };
        foundedCart.TotalPrice=await _cartRepository.GetCartPrice(id);
        return foundedCart;
    }

    public async Task<decimal> GetCartPrice(int id)
    {
        return await _cartRepository.GetCartPrice(id);
    }

    public async Task CreateOrder(int cartId,string address,DateTime? byTheTime)
    {
        var cart=await _cartRepository.GetCartById(cartId);
        if (cart==null || cart.Products.Count == 0)
            throw new BadHttpRequestException("Нельзя создать заказ с пустой корзиной");
        OrderRequestDto orderRequestDto = new OrderRequestDto(cartId,cart.Products,await _cartRepository.GetCartPrice(cartId),address,byTheTime);
    }

    public async Task<CartProduct> AddProductToCart(int cartId, int productId, int quantity)
    {
        var cart = await _cartRepository.GetCartById(cartId) ?? await _cartRepository.CreateCart(cartId);
        var existingProduct = await _cartRepository.GetProductById(cartId, productId);
        if (existingProduct != null)
            return await _cartRepository.UpdateProductQuantity(cartId,productId,quantity);
        else
            return await _cartRepository.AddProduct(cartId, productId, quantity);
    }

    public async Task<CartProduct> UpdateProductQuantity(int cartId, int productId, int quantity)
    {
        return await _cartRepository.UpdateProductQuantity(cartId,productId,quantity);
    }

    public async Task DeleteProductInCart(int cartId, int productId)
    {
        await _cartRepository.DeleteProductInCart(cartId, productId);
    }

    public void AddProductInfo(ProductShortInfoDto product)
    {
        _cacheManager.AddProductInfo(product);
    }

    public void UpdateProductInfo(ProductShortInfoDto product)
    {
        _cacheManager.UpdateProductInfo(product);
    }

    public void DeleteProductInfo(ProductShortInfoDto product)
    {
        _cacheManager.DeleteProductInfo(product.Id);
    }
}