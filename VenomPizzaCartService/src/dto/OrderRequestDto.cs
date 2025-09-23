using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.dto;

public class OrderRequestDto
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public List<OrderProductDto> ProductList { get; set; }
    public decimal Price { get; set; }
    public string Address { get; set; }
    public DateTime OrderedTime { get; set; } = DateTime.Now;
    public DateTime? ByTheTime { get; set; }
    public OrderRequestDto(int userId, List<OrderProductDto> productList, decimal price, string address, DateTime? byTheTime)
    {
        Id=Guid.NewGuid().ToString();
        UserId = userId;
        ProductList = productList;
        Price = price;
        Address = address;
        ByTheTime=byTheTime;
    }
}
