using Amazon.Runtime.Credentials;
using Microsoft.AspNetCore.Mvc;
using VenomPizzaCartService.src.attribute;
using VenomPizzaCartService.src.dto;
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

    [ValidateUserId]
    [HttpGet]
    public async Task<IActionResult> GetCartById()
    {
        int userId = (int)HttpContext.Items["Id"]!;
        return Ok(await _cartService.GetCartById(userId));
    }

    [ValidateUserId]
    [HttpPost("order")]
    public async Task<IActionResult> CreateOrder(CreateOrderRequestDto dto)
    {
        int userId = (int)HttpContext.Items["Id"]!;
        try
        {
            var orderRequest = await _cartService.CreateOrder(userId, dto.Address, dto.ByTheTime);
            return Ok(orderRequest);
        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [ValidateUserId]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductQuantity([FromRoute] int id, [FromRoute] int priceId, [FromQuery] int quantity)
    {
        int userId = (int)HttpContext.Items["Id"]!;
        try
        {
            return Ok(await _cartService.UpdateProductQuantity(userId, id, priceId, quantity));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [ValidateUserId]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductInCart([FromRoute] int id, [FromRoute] int priceId)
    {
        int userId = (int)HttpContext.Items["Id"]!;
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
