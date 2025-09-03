namespace VenomPizzaCartService.src.dto;

public class CartProductDto
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public int Quantity { get; set; } = 1;
}
