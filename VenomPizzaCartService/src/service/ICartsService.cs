using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public interface ICartsService
{
    Task<Cart> GetCartById(int id);
    Task<OrderRequestDto> CreateOrder(int cartId, string address, DateTime? byTheTime);
    Task<CartProduct> AddProductToCart(int cartId, int productId, int priceId, int quantity);
    Task<CartProduct> UpdateProductQuantity(int cartId, int productId, int priceId, int quantity);
    Task DeleteProductInCart(int cartId, int priceId, int productId);
    Task AddProductInfo(ProductShortInfoDto product);
    Task UpdateProductInfo(ProductShortInfoDto product);
    Task DeleteProductInfo(ProductShortInfoDto product);
}
