namespace VenomPizzaCartService.src.dto;

public class CartProductDto
{
    public int CartId { get; set; }
    public int Id { get; set; }
    public int Quantity { get; set; } = 1;

    public CartProductDto(int cartId, int id, int quantity)
    {
        CartId = cartId;
        Id = id;
        Quantity = quantity;
    }
}
