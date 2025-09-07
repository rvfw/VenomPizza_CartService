using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace VenomPizzaCartService.src.service;

public interface ICartsService
{
    Task<Cart> GetCartById(int id);
    Task<CartProduct> AddProductToCart(int cartId,int productId,int quantity);
    Task<CartProduct> UpdateProductQuantity(int cartId, int productId, int quantity);
    Task DeleteProductInCart(int cartId, int productId);
    void AddProductInfo(ProductShortInfoDto product);
    void UpdateProductInfo(ProductShortInfoDto product);
    void DeleteProductInfo(ProductShortInfoDto product);
}
