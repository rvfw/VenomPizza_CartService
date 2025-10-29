using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.dto;

public class CartProductDto
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int PriceId { get; set; }
    public int Quantity { get; set; } = 1;
    public string Title { get; set; }
    public string? Image { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }

    public CartProductDto(CartProduct cartProduct)
    {
        CartId = cartProduct.CartId;
        ProductId = cartProduct.ProductId;
        PriceId = cartProduct.PriceId;
        Quantity = cartProduct.Quantity;
    }

    public void AddInfo(string title, decimal price, string? image=null, bool isAvailable=true)
    {
        Title = title;
        Price = price;
        Image = image;
        IsAvailable = isAvailable;
    }
}
