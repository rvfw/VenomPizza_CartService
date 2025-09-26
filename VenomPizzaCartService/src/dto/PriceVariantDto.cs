namespace VenomPizzaCartService.src.dto;

public class PriceVariantDto
{
    public int PriceId { get; set; }
    public string Size { get; set; }
    public decimal Price { get; set; }
    public PriceVariantDto(int priceId, string size, decimal price)
    {
        PriceId = priceId;
        Size = size;
        Price = price;
    }
}
