using Microsoft.EntityFrameworkCore;
using Moq;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;
using EntityFrameworkCore.Testing.Moq;
using VenomPizzaCartService.src.dto;

namespace tests;

public class CartsRepositoryTest
{
    private readonly CartsDbContext _context;
    private readonly ICartsRepository _repository;
    private readonly Cart _validCart;
    public readonly CartProductDto _validProduct = new CartProductDto() { UserId=1, Id = 2, Quantity = 3 };
    public CartsRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<CartsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new CartsDbContext(options);
        _repository = new CartsRepository(_context);
        _validCart = new Cart { UserId = 1, Products = { new CartProduct() { ProductId = 1 } } };
    }

    [Fact]
    public async Task GetCartByUserId_Success()
    {
        var userId = 1;
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        var result = await _repository.GetCartByUserId(userId);
        var existingCart=_context.Carts.FirstOrDefault(c => c.UserId== userId);

        Assert.NotNull(existingCart);
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetCartByUserId_NotFound()
    {
        var userId = 1;

        var result = await _repository.GetCartByUserId(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetCartByUserId_IncludesProducts()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();
        var userId = 1;

        var result = await _repository.GetCartByUserId(userId);

        Assert.NotNull(result.Products);
        Assert.Single(result.Products);
    }

    [Fact]
    public async Task AddProductToCart_SuccessAdded()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();
        await _repository.AddProductToCart(_validProduct);

        var cart = _context.Carts.FirstOrDefault(x => x.UserId == 1)!;

        Assert.Equal(2, cart.Products.Count);
        Assert.NotNull(cart.Products.FirstOrDefault(x => x.ProductId == 2));
        Assert.Equal(3, cart.Products.FirstOrDefault(x => x.ProductId == 2)!.Quantity);
    }

    [Fact]
    public async Task AddProductToCart_SuccessCreateNewCartAndProduct()
    {
        await _repository.AddProductToCart(_validProduct);

        var cart = _context.Carts.FirstOrDefault(x => x.UserId == 1);

        Assert.NotNull(cart);
        Assert.Single(cart.Products);
        Assert.NotNull(cart.Products.FirstOrDefault(x => x.ProductId == 2));
        Assert.Equal(3,cart.Products.FirstOrDefault(x => x.ProductId == 2)!.Quantity);
    }

    [Fact]
    public async Task AddProductToCart_IncreaseQuantity()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();
        _validProduct.Id=1;
        await _repository.AddProductToCart(_validProduct);

        var cart = _context.Carts.FirstOrDefault(x => x.UserId == 1)!;

        Assert.Single(cart.Products);
        Assert.NotNull(cart.Products.FirstOrDefault(x => x.ProductId == 1));
        Assert.Equal(4,cart.Products.FirstOrDefault(x => x.ProductId == 1)!.Quantity);
    }

    [Fact]
    public async Task DeleteProductInCart_Success()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        Exception? ex=await Record.ExceptionAsync(()=> _repository.DeleteProductInCart(new() {UserId=1, Id = 1 }));

        Assert.Null(ex);
        Assert.Empty(_context.Carts.FirstOrDefault(x => x.UserId == 1)!.Products);
    }

    [Fact]
    public async Task DeleteProductInCart_CartNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(async() => await _repository.DeleteProductInCart(new() { UserId = 1, Id = 1 }));
    }

    [Fact]
    public async Task DeleteProductInCart_ProductNotFound()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.DeleteProductInCart(new() { UserId = 1, Id = 2 }));
    }
}
