using System.ComponentModel.DataAnnotations.Schema;

namespace VenomPizzaCartService.src.model;
[Table("carts")]
public class Cart
{
    [Column("id")]
    public int Id { get; set; }
    public List<CartProduct> Products { get; set; }=new List<CartProduct>();
    [Column("total_price")]
    public decimal TotalPrice { get; set; }
    public Cart() { }
}
