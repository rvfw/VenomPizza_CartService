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

    [HttpPost("{id}")]
    public async Task<IActionResult> AddProductInCart([FromRoute]int id,[FromQuery] int quantity=1)
    {
        int userId = int.Parse(Request.Headers["Id"].ToString());
        var createdProduct = await _cartService.AddProductToCart(userId,id,quantity);
        return CreatedAtAction(null,createdProduct.CartId,createdProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductQuantity([FromRoute] int id, [FromQuery] int quantity)
    {
        int userId = int.Parse(Request.Headers["Id"].ToString());
        return Ok(await _cartService.UpdateProductQuantity(userId, id, quantity));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductInCart([FromRoute] int id)
    {
        int userId = int.Parse(Request.Headers["Id"].ToString());
        await _cartService.DeleteProductInCart(userId, id);
        return Ok();
    }
}
