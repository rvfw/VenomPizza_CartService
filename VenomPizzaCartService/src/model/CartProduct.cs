using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenomPizzaCartService.src.model;
[Table("products")]
public class CartProduct
{
    [Column("product_id")]
    public int ProductId { get; set; }
    [Column("cart_id")]
    public int CartId { get; set; }
    public Cart Cart { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; } = 1;
    public CartProduct() { }
    public CartProduct(Cart cart,int productId,int quantity) { 
        Cart = cart;
        ProductId = productId;
        Quantity = quantity;
    }
}
