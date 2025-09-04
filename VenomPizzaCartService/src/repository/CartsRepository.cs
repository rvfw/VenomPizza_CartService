using Microsoft.EntityFrameworkCore;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.repository;

public class CartsRepository:ICartsRepository
{
    private readonly CartsDbContext _context;
    public CartsRepository(CartsDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartById(int id)
    {
        return await _context.Carts.Include(cp => cp.Products).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CartProduct?> GetProductById(int cartId,int productId)
    {
        return await _context.CartProducts.FirstOrDefaultAsync(x=>x.CartId== cartId && x.ProductId==productId);
    }
    public async Task<Cart> CreateCart(int cartId)
    {
        var cart= _context.Carts.Add(new Cart {Id = cartId }).Entity;
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartProduct> AddProduct(int cartId, int productId, int quantity)
    {
        var createdProduct=_context.CartProducts.Add(new CartProduct(cartId,productId,quantity)).Entity;
        await _context.SaveChangesAsync();
        return createdProduct;
    }

    public async Task<CartProduct> UpdateProductQuantity(int cartId,int productId,int quantity)
    {
        var foundedProduct = await _context.CartProducts.FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт с ID {productId} не найден");
        foundedProduct.Quantity = quantity;
        await _context.SaveChangesAsync();
        return foundedProduct;
    }

    public async Task DeleteProductInCart(int cartId,int productId)
    {
        var foundedProduct=await _context.CartProducts.FirstOrDefaultAsync(x=>x.CartId==cartId&&x.ProductId==productId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт с ID {productId} не найден");
        _context.CartProducts.Remove(foundedProduct);
        await _context.SaveChangesAsync();
    }
}
