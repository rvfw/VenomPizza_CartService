using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.dto;

public class OrderRequestDto
{

    public int UserId { get; set; }
    public List<CartProduct> ProductList { get; set; }
    public decimal Price { get; set; }
    public string Address { get; set; }
    public DateTime OrderedTime { get; set; } = DateTime.Now;
    public DateTime? ByTheTime { get; set; }
    public OrderRequestDto(int userId, List<CartProduct> productList, decimal price, string address, DateTime? byTheTime)
    {
        UserId = userId;
        ProductList = productList;
        Price = price;
        Address = address;
        ByTheTime=byTheTime;
    }
}
