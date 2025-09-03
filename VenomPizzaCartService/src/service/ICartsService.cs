using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public interface ICartsService
{
    Task<Cart> GetCartByUserId(int userId);
    Task AddProductToCart(CartProductDto dto);
    Task UpdateProductQuantity(CartProductDto dto);
    Task DeleteProductInCart(CartProductDto dto);
}
