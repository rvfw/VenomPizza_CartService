using Microsoft.EntityFrameworkCore;
using Moq;
using VenomPizzaCartService.src.context;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.repository;
using VenomPizzaCartService.src.service;

namespace tests;

public class CartsServiceTest
{
    private readonly CartsService _service;
    private readonly Mock<ICartsRepository> _mockRepository;
    private readonly Cart _validCart;
    public CartsServiceTest()
    {
        _mockRepository = new Mock<ICartsRepository>();
        _service = new CartsService(_mockRepository.Object);
        _validCart = new Cart { Id = 1, Products = { new CartProduct() { ProductId = 1 } } };
    }

    #region create
    [Fact]
    public async Task AddProductToCart_AddProduct()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(new Cart() { Id = 1 });
        _mockRepository.Setup(rep => rep.GetProductById(1, 2)).ReturnsAsync(() => null);
        _mockRepository.Setup(rep => rep.AddProduct(1, 2, 3)).ReturnsAsync(new CartProduct(1, 2, 3));

        var res = await _service.AddProductToCart(1, 2, 3);

        _mockRepository.Verify(rep => rep.GetCartById(1), Times.Once());
        _mockRepository.Verify(rep => rep.GetProductById(1, 2), Times.Once());
        _mockRepository.Verify(rep => rep.AddProduct(1, 2, 3), Times.Once());
        Assert.NotNull(res);
        Assert.Equal(1, res.CartId);
        Assert.Equal(2, res.ProductId);
        Assert.Equal(3, res.Quantity);
    }

    [Fact]
    public async Task AddProductToCart_CreateCartAndAddProduct()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(() => null);
        _mockRepository.Setup(rep => rep.CreateCart(1)).ReturnsAsync(new Cart() { Id = 1 });
        _mockRepository.Setup(rep => rep.GetProductById(1, 2)).ReturnsAsync(() => null);
        _mockRepository.Setup(rep => rep.AddProduct(1, 2, 3)).ReturnsAsync(new CartProduct(1, 2, 3));

        var res = await _service.AddProductToCart(1, 2, 3);

        _mockRepository.Verify(rep => rep.GetCartById(1), Times.Once());
        _mockRepository.Verify(rep => rep.GetProductById(1, 2), Times.Once());
        _mockRepository.Verify(rep => rep.CreateCart(1), Times.Once());
        _mockRepository.Verify(rep => rep.AddProduct(1, 2, 3), Times.Once());
        Assert.NotNull(res);
        Assert.Equal(1, res.CartId);
        Assert.Equal(2, res.ProductId);
        Assert.Equal(3, res.Quantity);
    }

    [Fact]
    public async Task AddProductToCart_UpdateProductQuantity()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(new Cart() { Id = 1 });
        _mockRepository.Setup(rep => rep.GetProductById(1, 2)).ReturnsAsync(() => new CartProduct(1, 2, 3));
        _mockRepository.Setup(rep => rep.UpdateProductQuantity(1, 2, 3)).ReturnsAsync(new CartProduct(1, 2, 6));

        var res = await _service.AddProductToCart(1, 2, 3);

        _mockRepository.Verify(rep => rep.GetCartById(1), Times.Once());
        _mockRepository.Verify(rep => rep.GetProductById(1, 2), Times.Once());
        _mockRepository.Verify(rep => rep.UpdateProductQuantity(1, 2, 3), Times.Once());
        Assert.NotNull(res);
        Assert.Equal(1, res.CartId);
        Assert.Equal(2, res.ProductId);
        Assert.Equal(6, res.Quantity);
    } 
    #endregion

    #region read
    [Fact]
    public async Task GetCartById_ExistingCart()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(_validCart);

        var res = await _service.GetCartById(1);

        Assert.NotNull(res);
        Assert.Equal(1, res.Id);
        Assert.Single(res.Products);
        Assert.Equal(1, res.Products[0].ProductId);
        _mockRepository.Verify(repo => repo.GetCartById(1), Times.Once());
    }

    [Fact]
    public async Task GetCartById_NewCart()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync((Cart)null);

        var res = await _service.GetCartById(1);

        Assert.NotNull(res);
        Assert.Equal(1, res.Id);
        Assert.Empty(res.Products);
        _mockRepository.Verify(rep => rep.GetCartById(1), Times.Once());
    }
    #endregion

    #region update
    [Fact]
    public async Task UpdateProductQuantity_Success()
    {
        _mockRepository.Setup(rep => rep.UpdateProductQuantity(1, 2, 3)).ReturnsAsync(new CartProduct(1, 2, 6));

        var res = await _service.UpdateProductQuantity(1, 2, 3);

        _mockRepository.Verify(rep => rep.UpdateProductQuantity(1, 2, 3), Times.Once());
        Assert.Equal(6, res.Quantity);
    } 
    #endregion

}
