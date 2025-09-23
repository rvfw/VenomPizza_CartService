using System.Drawing;

namespace VenomPizzaCartService.src.dto;

public class OrderProductDto
{
    public int ProductId { get; set; }
    public string Title { get; set; }
    public string Size { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public OrderProductDto(int productId, string title, string size, string? imageUrl, int quantity, decimal price)
    {
        ProductId = productId;
        Title = title;
        Size = size;
        ImageUrl = imageUrl;
        Quantity = quantity;
        Price = price;
    }
}
