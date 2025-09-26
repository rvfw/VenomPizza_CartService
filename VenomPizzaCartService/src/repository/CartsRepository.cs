using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.etc;
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

    #region create
    public async Task<Cart> CreateCart(int cartId)
    {
        _logger.LogInformation($"Создаем корзину {cartId}");
        var cart = _context.Carts.Add(new Cart { Id = cartId }).Entity;
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartProduct> AddProduct(int cartId, int productId, int priceId, int quantity)
    {
        _logger.LogInformation($"Добавляем продукт {productId} в корзину {cartId} в кол-ве {quantity}");
        var createdProduct = _context.CartProducts.Add(new CartProduct(cartId, productId,priceId, quantity)).Entity;
        await _context.SaveChangesAsync();
        return createdProduct;
    }

    #endregion

    #region read
    public async Task<Cart?> GetCartById(int id)
    {
        _logger.LogInformation($"Получаем корзину {id}");
        return await _context.Carts.AsNoTracking().Include(cp => cp.Products).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CartProduct?> GetProductById(int cartId, int productId, int priceId)
    {
        _logger.LogInformation($"Получаем продукт {productId} с ценой {priceId} из корзины {cartId}");
        return await _context.CartProducts
            .FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId && x.PriceId==priceId);
    }
    #endregion

    #region update
    public async Task<CartProduct> UpdateProductQuantity(int cartId, int productId, int priceId, int quantity)
    {
        _logger.LogInformation($"Обновляем продукт {productId} в корзине {cartId} на новое кол-во: {quantity}");
        var foundedProduct = await GetProductById(cartId, productId, priceId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт {productId} с ценой {priceId} не найден в корзине {cartId}");
        foundedProduct.Quantity = quantity;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Обновленный продукт {productId} с ценой {priceId}. Новое кол-во: {quantity}");
        return foundedProduct;
    }
    #endregion

    #region delete
    public async Task DeleteProductInCart(int cartId, int productId, int priceId)
    {
        _logger.LogInformation($"Из корзины {cartId} удаляем продукт {productId}");
        var foundedProduct = await GetProductById(cartId,productId,priceId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт {productId} и ценой {priceId} не найден в корзине {cartId}");
        _context.CartProducts.Remove(foundedProduct);
        _logger.LogInformation($"Продукт {productId} с ценой {priceId} удален из корзины {cartId}");
        await _context.SaveChangesAsync();
    } 
    #endregion

}
