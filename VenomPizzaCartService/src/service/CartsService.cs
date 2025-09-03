using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public class CartsService:ICartsService
{
    private readonly ICartsRepository _cartRepository;
    public CartsService(CartsRepository repository)
    {
        _cartRepository = repository;
    }
    public async Task<Cart> GetCartByUserId(int userId)
    {
        return await _cartRepository.GetCartByUserId(userId);
    }
    public async Task AddProductToCart(CartProductDto dto)
    {
        await _cartRepository.AddProductToCart(dto);
    }
    public async Task UpdateProductQuantity(CartProductDto dto)
    {
        await _cartRepository.UpdateProductQuantity(dto);
    }
    public async Task DeleteProductInCart(CartProductDto dto)
    {
        await _cartRepository.DeleteProductInCart(dto);
    }
}