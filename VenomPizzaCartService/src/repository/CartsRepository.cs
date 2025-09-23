using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
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
    private readonly Timer _snapshotTimer;
    private readonly ICacheManager _cacheManager;
    public CartsRepository(CartsDbContext context,ILogger<CartsRepository> logger, ICacheManager cacheManager)
    {
        _context = context;
        _logger = logger;
        _cacheManager = cacheManager;
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

    public async Task<CartProduct?> GetProductById(int cartId, int productId)
    {
        _logger.LogInformation($"Получаем продукт {productId} из корзины {cartId}");
        return await _context.CartProducts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);
    }

    public async Task<decimal> GetCartPrice(int cartId)
    {
        _logger.LogInformation($"Подсчет цены товаров из корзины {cartId}");
        var cart = await GetCartById(cartId);
        decimal sum = 0;
        foreach (var productInCart in cart.Products)
        {
            var foundedProduct = _cacheManager.GetProductCacheById(productInCart.ProductId);
            if (foundedProduct == null)
                throw new KeyNotFoundException($"Не найдены цены продукта {productInCart.ProductId}");
            var priceVariant = foundedProduct.Prices.FirstOrDefault(x => x.PriceId == productInCart.PriceId);
            if(priceVariant==null)
                throw new KeyNotFoundException($"Не найдена цена {productInCart.PriceId} продукта {productInCart.ProductId}");
            sum += priceVariant.Price * productInCart.Quantity;
        }
        _logger.LogInformation($"Цена товаров из корзины {cartId}: {sum}");
        return sum;
    }
    #endregion

    #region update
    public async Task<CartProduct> UpdateProductQuantity(int cartId, int productId, int priceId, int quantity)
    {
        _logger.LogInformation($"Обновляем продукт {productId} в корзину {cartId} на новое кол-во: {quantity}");
        var foundedProduct = await _context.CartProducts.FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт с ID {productId} не найден в корзине с ID {cartId}");
        foundedProduct.Quantity = quantity;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Обновленный продукт {productId}. Новое кол-во: {quantity}");
        return foundedProduct;
    }
    #endregion

    #region delete
    public async Task DeleteProductInCart(int cartId, int productId, int priceId)
    {
        _logger.LogInformation($"Из корзины {cartId} удаляем продукт {productId}");
        var foundedProduct = await _context.CartProducts.FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);
        if (foundedProduct == null)
            throw new KeyNotFoundException($"Продукт с ID {productId} не найден в корзине с ID {cartId}");
        _context.CartProducts.Remove(foundedProduct);
        _logger.LogInformation($"Продукт {productId} удален из корзины {cartId}");
        await _context.SaveChangesAsync();
    } 
    #endregion

}
