using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.dto;

public class CartDto
{
    public int Id { get; set; }
    public List<CartProductDto> Products { get; set; }=new();
    public decimal TotalPrice { get; set; }

    public CartDto()
    {
        
    }
    
    public CartDto(int id)
    {
        Id = id;
    }
    
    public CartDto(Cart cart)
    {
        Id = cart.Id;
        Products = cart.Products.Select(product => new CartProductDto(product)).ToList();
    }
}