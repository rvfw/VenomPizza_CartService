using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.repository;

public class CartsRepository
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
    public async Task AddProductToCart(int userId, (int id, int quantity) cartProduct)
    {
        var cart = await _context.Carts.Include(cp => cp.Products).FirstOrDefaultAsync(x => x.UserId == userId);
        CartProduct? existingProduct= null;
        if (cart == null)
        {
            var createdCart = _context.Carts.Add(new Cart() { UserId = userId }).Entity;
            await _context.SaveChangesAsync();
            cart = createdCart;
        }
        else
            existingProduct = _context.CartProducts.FirstOrDefault(cp => cp.CartId == cart.UserId && cp.ProductId == cartProduct.id);
        if (existingProduct != null)
            existingProduct.Quantity++;
        else
            _context.CartProducts.Add(new CartProduct(cart, cartProduct.id, cartProduct.quantity));
        await _context.SaveChangesAsync();
    }
    public async Task UpdateProductQuantity(int userId, (int id, int quantity) cartProduct)
    {
        var cart = await _context.Carts.Include(cp => cp.Products).FirstOrDefaultAsync(x => x.UserId == userId);
        CartProduct? existingProduct = _context.CartProducts.FirstOrDefault(cp => cp.CartId == cart.UserId && cp.ProductId == cartProduct.id);
        if (existingProduct == null)
            throw new KeyNotFoundException("Продукт не найден в корзине");
        existingProduct.Quantity=cartProduct.quantity;
        await _context.SaveChangesAsync();
    }
}
