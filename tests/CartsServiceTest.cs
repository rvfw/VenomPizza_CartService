using Confluent.Kafka;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.etc;
using VenomPizzaCartService.src.Kafka;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.providers;
using VenomPizzaCartService.src.repository;
using VenomPizzaCartService.src.service;

namespace tests;

public class CartsServiceTest
{
    private readonly CartsService _service;
    private readonly Mock<ICartsRepository> _mockRepository;
    private readonly Mock<ICloudStorageProvider> _mockCloudStorageProvider;
    private readonly Cart _validCart;
    public CartsServiceTest()
    {
        _mockCloudStorageProvider = new Mock<ICloudStorageProvider>();
        _mockRepository = new Mock<ICartsRepository>();
        _service = new CartsService(_mockRepository.Object,_mockCloudStorageProvider.Object, 
            NullLogger<CartsService>.Instance, new Mock<IProducer<string,string>>().Object, new Mock<IOptions<KafkaSettings>>().Object,new Mock<ICacheProvider>().Object);
        _validCart = new Cart { Id = 1, Products = { new CartProduct() { ProductId = 1 } } };
    }

    #region create
    [Fact]
    public async Task AddProductToCart_AddProduct()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(new Cart() { Id = 1 });
        _mockRepository.Setup(rep => rep.GetProductById(1, 2,3)).ReturnsAsync(()=>null);
        _mockRepository.Setup(rep => rep.AddProduct(1, 2, 3, 4)).ReturnsAsync(new CartProduct(1, 2, 3, 4));

        var res = await _service.AddProductToCart(1, 2, 3, 4);

        _mockRepository.Verify(rep => rep.GetCartById(1), Times.Once());
        _mockRepository.Verify(rep => rep.GetProductById(1, 2,3), Times.Once());
        _mockRepository.Verify(rep => rep.AddProduct(1, 2, 3, 4), Times.Once());
        Assert.NotNull(res);
        Assert.Equal(1, res.CartId);
        Assert.Equal(2, res.ProductId);
        Assert.Equal(3, res.PriceId);
        Assert.Equal(4, res.Quantity);
    }

    [Fact]
    public async Task AddProductToCart_CreateCartAndAddProduct()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(() => null);
        _mockRepository.Setup(rep => rep.CreateCart(1)).ReturnsAsync(new Cart() { Id = 1 });
        _mockRepository.Setup(rep => rep.GetProductById(1, 2,3)).ReturnsAsync(() => null);
        _mockRepository.Setup(rep => rep.AddProduct(1, 2, 3, 4)).ReturnsAsync(new CartProduct(1, 2, 3, 4));

        var res = await _service.AddProductToCart(1, 2, 3, 4);

        _mockRepository.Verify(rep => rep.GetCartById(1), Times.Once());
        _mockRepository.Verify(rep => rep.GetProductById(1, 2,3), Times.Once());
        _mockRepository.Verify(rep => rep.CreateCart(1), Times.Once());
        _mockRepository.Verify(rep => rep.AddProduct(1, 2, 3, 4), Times.Once());
        Assert.NotNull(res);
        Assert.Equal(1, res.CartId);
        Assert.Equal(2, res.ProductId);
        Assert.Equal(3, res.PriceId);
        Assert.Equal(4, res.Quantity);
    }

    [Fact]
    public async Task AddProductToCart_UpdateProductQuantity()
    {
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(new Cart() { Id = 1 });
        _mockRepository.Setup(rep => rep.GetProductById(1, 2,3)).ReturnsAsync(() => new CartProduct(1, 2, 3, 4));
        _mockRepository.Setup(rep => rep.UpdateProductQuantity(1, 2, 3, 99)).ReturnsAsync(new CartProduct(1, 2, 3, 99));

        var res = await _service.AddProductToCart(1, 2, 3, 99);

        _mockRepository.Verify(rep => rep.GetCartById(1), Times.Once());
        _mockRepository.Verify(rep => rep.GetProductById(1, 2,3), Times.Once());
        _mockRepository.Verify(rep => rep.UpdateProductQuantity(1, 2, 3, 99), Times.Once());
        Assert.NotNull(res);
        Assert.Equal(1, res.CartId);
        Assert.Equal(2, res.ProductId);
        Assert.Equal(3, res.PriceId);
        Assert.Equal(99, res.Quantity);
    }
    #endregion

    #region read
    [Fact]
    public async Task GetCartById_ExistingCart()
    {
        var cachedProducts = new List<ProductShortInfoDto>() { new ProductShortInfoDto(1, "") 
        { Prices = new List<PriceVariantDto>() { new PriceVariantDto(0, "Стандартная", 1) } } };
        _mockRepository.Setup(rep => rep.GetCartById(1)).ReturnsAsync(_validCart);
        _mockCloudStorageProvider.Setup(cloud => cloud.GetProductsCacheById(new List<int>() { 1 })).Returns(cachedProducts);

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
        _mockRepository.Setup(rep => rep.UpdateProductQuantity(1, 2, 3, 99)).ReturnsAsync(new CartProduct(1, 2, 3, 99));

        var res = await _service.UpdateProductQuantity(1, 2, 3, 99);

        _mockRepository.Verify(rep => rep.UpdateProductQuantity(1, 2, 3, 99), Times.Once());
        Assert.Equal(3, res.PriceId);
        Assert.Equal(99, res.Quantity);
    } 
    #endregion

}
