using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VenomPizzaCartService.src.model;
[Table("products")]
public class CartProduct
{
    [Column("product_id")]
    public int ProductId { get; set; }
    [Column("cart_id")]
    public int CartId { get; set; }
    [Column("price_id")]
    public int PriceId { get; set; }
    [JsonIgnore]
    public Cart Cart { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; } = 1;
    public CartProduct() { }
    public CartProduct(int cartId, int productId,int priceId, int quantity)
    {
        CartId=cartId;
        ProductId = productId;
        PriceId = priceId;
        Quantity = quantity;
    }
}
