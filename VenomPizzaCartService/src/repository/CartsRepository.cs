using Microsoft.EntityFrameworkCore;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.repository;

public class CartsRepository:ICartsRepository
{
    private readonly CartsDbContext _context;
    public CartsRepository(CartsDbContext context)
    {
        _context = context;
    }

    public async Task<Cart> GetCartByUserId(int userId)
    {
        var foundedCart= await _context.Carts.Include(cp=>cp.Products).FirstOrDefaultAsync(x => x.UserId == userId);
        if (foundedCart == null)
            return new Cart() { UserId = userId };
        return foundedCart;
    }

    public async Task AddProductToCart(CartProductDto dto)
    {
        var cart = await _context.Carts.Include(cp => cp.Products).FirstOrDefaultAsync(x => x.UserId == dto.UserId);
        CartProduct? existingProduct= null;
        if (cart == null)
        {
            var createdCart = _context.Carts.Add(new Cart() { UserId = dto.UserId }).Entity;
            await _context.SaveChangesAsync();
            cart = createdCart;
        }
        else
            existingProduct = _context.CartProducts.FirstOrDefault(cp => cp.CartId == cart.UserId && cp.ProductId == dto.Id);
        if (existingProduct != null)
            existingProduct.Quantity+=dto.Quantity;
        else
            _context.CartProducts.Add(new CartProduct(cart, dto.Id, dto.Quantity));
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductQuantity(CartProductDto dto)
    {
        var cart = await _context.Carts.Include(cp => cp.Products).FirstOrDefaultAsync(x => x.UserId == dto.UserId);
        CartProduct? existingProduct = _context.CartProducts.FirstOrDefault(cp => cp.CartId == cart.UserId && cp.ProductId == dto.Id);
        if (existingProduct == null)
            throw new KeyNotFoundException("Продукт не найден в корзине");
        existingProduct.Quantity=dto.Quantity;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductInCart(CartProductDto dto)
    {
        var cart = await _context.Carts.Include(cp => cp.Products).FirstOrDefaultAsync(x => x.UserId == dto.UserId);
        if (cart == null)
            throw new KeyNotFoundException($"Корзина пользователя {dto.UserId} не найдена");
        CartProduct? existingProduct = _context.CartProducts.FirstOrDefault(cp => cp.CartId == cart.UserId && cp.ProductId == dto.Id);
        if (existingProduct == null)
            throw new KeyNotFoundException("Продукт не найден в корзине");
        _context.CartProducts.Remove(existingProduct);
        await _context.SaveChangesAsync();
    }
}
