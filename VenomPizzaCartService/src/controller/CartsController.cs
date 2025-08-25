using Microsoft.AspNetCore.Mvc;
using VenomPizzaCartService.src.service;

namespace VenomPizzaCartService.src.controller;

[ApiController]
[Route("api/cart")]
public class CartsController:Controller
{
    private readonly CartsService _cartService;
    public CartsController(CartsService service)
    {
        _cartService = service;
    }
    [HttpGet]
    public async Task<IActionResult> GetCartByUserId()
    {
        int userId;
        try
        {
            userId = int.Parse(Request.Headers["Id"].ToString());
            if (userId <= 0)
                throw new Exception();
        }
        catch (Exception ex) 
        { return BadRequest("Неверный формат ID"); }
        return Ok(await _cartService.GetCartByUserId(userId));
    }
}
