namespace VenomPizzaCartService.src.dto;

public class ProductShortInfoDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public decimal Price { get; set; }
    public ProductShortInfoDto(int id, string title)
    {
        Id = id;
        Title = title;
    }
}
