using Microsoft.EntityFrameworkCore.Query;

namespace VenomPizzaCartService.src.dto;

public class CartProductDto
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int PriceId { get; set; }
    public int Quantity { get; set; } = 1;

    public CartProductDto(int cartId, int productId,int priceId, int quantity)
    {
        CartId = cartId;
        ProductId = productId;
        PriceId = priceId;
        Quantity = quantity;
    }
}
