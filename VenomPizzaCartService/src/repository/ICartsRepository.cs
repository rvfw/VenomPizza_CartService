using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.repository;

public interface ICartsRepository
{
    Task<Cart> GetCartByUserId(int userId);
    Task AddProductToCart(CartProductDto dto);
    Task UpdateProductQuantity(CartProductDto dto);
    Task DeleteProductInCart(CartProductDto dto);
}
