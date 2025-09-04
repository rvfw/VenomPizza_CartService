using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.repository;

public interface ICartsRepository
{
    Task<Cart> CreateCart(int cartId);
    Task<Cart?> GetCartById(int cartId);
    Task<CartProduct?> GetProductById(int cartId,int productId);
    Task<CartProduct> AddProduct(int cartId,int productId,int quantity);
    Task<CartProduct> UpdateProductQuantity(int cartId, int productId, int quantity);
    Task DeleteProductInCart(int cartId,int productId);
}
