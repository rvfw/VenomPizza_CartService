namespace VenomPizzaCartService.src.model;

public class Cart
{
    public int UserId { get; set; }
    public List<CartProduct> Products { get; set; }=new List<CartProduct>();
    public decimal TotalPrice { get; set; }
    public Cart() { }
}
