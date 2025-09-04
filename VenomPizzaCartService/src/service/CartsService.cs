using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public class CartsService:ICartsService
{
    private readonly ICartsRepository _cartRepository;
    public CartsService(ICartsRepository repository)
    {
        _cartRepository = repository;
    }
    public async Task<Cart> GetCartById(int id)
    {
        var foundedCart= await _cartRepository.GetCartById(id);
        return foundedCart ?? new Cart() { Id = id };
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
}