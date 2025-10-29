using System.ComponentModel.DataAnnotations.Schema;

namespace VenomPizzaCartService.src.model;
[Table("carts")]
public class Cart
{
    [Column("id")]
    public int Id { get; init; }
    public List<CartProduct> Products { get; set; }=new();
    public Cart() { }
}
