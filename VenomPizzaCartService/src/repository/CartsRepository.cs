using Microsoft.EntityFrameworkCore;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.repository;

public class CartsRepository:ICartsRepository
{
    private readonly CartsDbContext _context;
    private readonly ILogger<CartsRepository> _logger;
    public CartsRepository(CartsDbContext context,ILogger<CartsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Cart?> GetCartById(int id)
    {
        _logger.LogInformation($"Получаем корзину {id}");
        return await _context.Carts.Include(cp => cp.Products).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CartProduct?> GetProductById(int cartId,int productId)
    {
        _logger.LogInformation($"Получаем продукт {productId} из корзины {cartId}");
        return await _context.CartProducts.FirstOrDefaultAsync(x=>x.CartId== cartId && x.ProductId==productId);
    }
    public async Task<Cart> CreateCart(int cartId)
    {
        _logger.LogInformation($"Создаем корзину {cartId}");
        var cart= _context.Carts.Add(new Cart {Id = cartId }).Entity;
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartProduct> AddProduct(int cartId, int productId, int quantity)
    {
        _logger.LogInformation($"Добавляем продукт {productId} в корзину {cartId} в кол-ве {quantity}");
        var createdProduct=_context.CartProducts.Add(new CartProduct(cartId,productId,quantity)).Entity;
        await _context.SaveChangesAsync();
        return createdProduct;
    }

    public async Task<CartProduct> UpdateProductQuantity(int cartId,int productId,int quantity)
    {
        _logger.LogInformation($"Обновляем продукт {productId} в корзину {cartId} на новое кол-во: {quantity}");
        var foundedProduct = await _context.CartProducts.FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт с ID {productId} не найден");
        foundedProduct.Quantity = quantity;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Обновленный продукт {productId}. Новое кол-во: {quantity}");
        return foundedProduct;
    }

    public async Task DeleteProductInCart(int cartId,int productId)
    {
        _logger.LogInformation($"Из корзины {cartId} удаляем продукт {productId}");
        var foundedProduct=await _context.CartProducts.FirstOrDefaultAsync(x=>x.CartId==cartId&&x.ProductId==productId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт с ID {productId} не найден");
        _context.CartProducts.Remove(foundedProduct);
        _logger.LogInformation($"Продукт {productId} удален из корзины {cartId}");
        await _context.SaveChangesAsync();
    }
}
