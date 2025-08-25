using System.ComponentModel.DataAnnotations;

namespace VenomPizzaCartService.src.model;

public class CartProduct
{
    public int ProductId { get; set; }
    public int CartId { get; set; }
    public Cart Cart { get; set; }
    public int Quantity {  get; set; }
    public CartProduct() { }
    public CartProduct(Cart cart,int productId,int quantity) { 
        Cart = cart;
        ProductId = productId;
        Quantity = quantity;
    }
}
