namespace VenomPizzaCartService.src.dto;

public class ProductShortInfoDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public string? Unit { get; set; }
    public List<PriceVariantDto> Prices { get; set; }
    public ProductShortInfoDto(int id, string title)
    {
        Id = id;
        Title = title;
    }
}
