using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public class CartsService
{
    private readonly CartsRepository _cartRepository;
    public CartsService(CartsRepository repository)
    {
        _cartRepository = repository;
    }
    public async Task<Cart> GetCartByUserId(int userId)
    {
        return await _cartRepository.GetCartByUserId(userId);
    }
    public async Task AddProductToCart(int userId, (int id, int quantity) cartProduct)
    {
        await _cartRepository.AddProductToCart(userId,cartProduct);
    }
    public async Task UpdateProductQuantity(int userId, (int id, int quantity) cartProduct)
    {
        await _cartRepository.UpdateProductQuantity(userId, cartProduct);
    }
}
