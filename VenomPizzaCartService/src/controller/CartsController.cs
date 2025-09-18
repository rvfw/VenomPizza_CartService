using Microsoft.AspNetCore.Mvc;
using VenomPizzaCartService.src.service;

namespace VenomPizzaCartService.src.controller;

[ApiController]
[Route("api/cart")]
public class CartsController:Controller
{
    private readonly ICartsService _cartService;
    public CartsController(ICartsService service)
    {
        _cartService = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCartById()
    {
        int userId = int.Parse(Request.Headers["Id"].ToString());
        return Ok(await _cartService.GetCartById(userId));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductQuantity([FromRoute] int id, [FromRoute] int priceId, [FromQuery] int quantity)
    {
        int userId = int.Parse(Request.Headers["Id"].ToString());
        try
        {
            return Ok(await _cartService.UpdateProductQuantity(userId, id, priceId, quantity));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductInCart([FromRoute] int id, [FromRoute] int priceId)
    {
        int userId = int.Parse(Request.Headers["Id"].ToString());
        try
        {
            await _cartService.DeleteProductInCart(userId, id, priceId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
