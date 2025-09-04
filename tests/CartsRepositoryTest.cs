using Microsoft.EntityFrameworkCore;
using Moq;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;

namespace tests;

public class CartsRepositoryTest
{
    private readonly CartsDbContext _context;
    private readonly ICartsRepository _repository;
    private readonly Cart _validCart;
    public CartsRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<CartsDbContext>()
            .UseInMemoryDatabase(databaseName:Guid.NewGuid().ToString())
            .Options;
        _context = new CartsDbContext(options);
        _repository = new CartsRepository(_context);
        _validCart = new Cart { Id = 1, Products = { new CartProduct() { ProductId = 1 } } };
    }

    #region create
    [Fact]
    public async Task AddProductToCart_SuccessAdded()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();
        await _repository.AddProduct(1,2,3);

        var cart = _context.Carts.FirstOrDefault(x => x.Id == 1)!;

        Assert.Equal(2, cart.Products.Count);
        Assert.NotNull(cart.Products.FirstOrDefault(x => x.ProductId == 2));
        Assert.Equal(3, cart.Products.FirstOrDefault(x => x.ProductId == 2)!.Quantity);
    }
    #endregion

    #region read
    [Fact]
    public async Task GetCartById_Success()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        var result = await _repository.GetCartById(1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCartById_NotFound()
    {
        var result = await _repository.GetCartById(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCartById_IncludesProducts()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        var result = await _repository.GetCartById(1);

        Assert.NotNull(result.Products);
        Assert.Single(result.Products);
    }

    [Fact]
    public async Task GetProductById_NotFound()
    {
        var foundedProduct = await _repository.GetProductById(1, 1);

        Assert.Null(foundedProduct);
    }
    #endregion

    #region update
    [Fact]
    public async Task UpdateProductQuantity_Success()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        await _repository.UpdateProductQuantity(1, 1, 5);
        var product = _context.Carts.FirstOrDefault(c => c.Id == 1)!.Products.FirstOrDefault(x => x.ProductId == 1)!;

        Assert.Equal(5, product.Quantity);
    }

    [Fact]
    public async Task UpdateProductQuantity_NotFound()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.UpdateProductQuantity(1, 2, 5));
    } 
    #endregion

    #region delete
    [Fact]
    public async Task DeleteProductInCart_Success()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        await _repository.DeleteProductInCart(1, 1);

        Assert.Empty(_context.Carts.FirstOrDefault(x => x.Id == 1)!.Products);
    }

    [Fact]
    public async Task DeleteProductInCart_NotFound()
    {
        _context.Add(_validCart);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(async()=> await _repository.DeleteProductInCart(1, 2));
    }
    #endregion
}
