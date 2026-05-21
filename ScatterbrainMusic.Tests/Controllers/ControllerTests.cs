using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ScatterbrainMusic.API.Controllers;
using ScatterbrainMusic.API.Models;
using ScatterbrainMusic.API.Services;

namespace ScatterbrainMusic.Tests.Controllers;

[TestFixture]
[Category("Unit")]
public class ProductsControllerTests
{
    private Mock<IProductService> _mockService = null!;
    private ProductsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductsController(_mockService.Object);
    }

    // ─── GetProducts ────────────────────────────────────────────────────────────

    [Test]
    public async Task GetProducts_ReturnsOk_WithProductsResponse()
    {
        var expected = new ProductsResponse
        {
            Products = new List<Product> { new() { Id = 1, Name = "Guitar", Price = 500m } },
            TotalCount = 1, Page = 1, PageSize = 12
        };
        _mockService.Setup(s => s.GetProductsAsync(null, null, 1, 12)).ReturnsAsync(expected);

        var result = await _controller.GetProducts();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task GetProducts_InvalidPage_ClampedToOne()
    {
        _mockService.Setup(s => s.GetProductsAsync(null, null, 1, 12))
                    .ReturnsAsync(new ProductsResponse());

        await _controller.GetProducts(page: -5);

        _mockService.Verify(s => s.GetProductsAsync(null, null, 1, 12), Times.Once);
    }

    [Test]
    public async Task GetProducts_OversizedPageSize_ClampedToTwelve()
    {
        _mockService.Setup(s => s.GetProductsAsync(null, null, 1, 12))
                    .ReturnsAsync(new ProductsResponse());

        await _controller.GetProducts(pageSize: 9999);

        _mockService.Verify(s => s.GetProductsAsync(null, null, 1, 12), Times.Once);
    }

    [Test]
    public async Task GetProducts_PassesCategoryAndSearch()
    {
        _mockService.Setup(s => s.GetProductsAsync("Guitars", "Fender", 1, 12))
                    .ReturnsAsync(new ProductsResponse());

        await _controller.GetProducts(category: "Guitars", search: "Fender");

        _mockService.Verify(s => s.GetProductsAsync("Guitars", "Fender", 1, 12), Times.Once);
    }

    // ─── GetProduct ─────────────────────────────────────────────────────────────

    [Test]
    public async Task GetProduct_ValidId_ReturnsOk()
    {
        var product = new Product { Id = 1, Name = "Test" };
        _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

        var result = await _controller.GetProduct(1);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(product);
    }

    [Test]
    public async Task GetProduct_InvalidId_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        var result = await _controller.GetProduct(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    // ─── GetCategories ──────────────────────────────────────────────────────────

    [Test]
    public async Task GetCategories_ReturnsOk_WithList()
    {
        var categories = new List<string> { "Accessories", "Gear", "Guitars", "Vinyl" };
        _mockService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(categories);

        var result = await _controller.GetCategories();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(categories);
    }

    // ─── GetFeatured ────────────────────────────────────────────────────────────

    [Test]
    public async Task GetFeatured_ReturnsOk_WithFeaturedList()
    {
        var products = new List<Product>
        {
            new() { Id = 1, IsFeatured = true },
            new() { Id = 2, IsFeatured = true }
        };
        _mockService.Setup(s => s.GetFeaturedProductsAsync()).ReturnsAsync(products);

        var result = await _controller.GetFeatured();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(products);
    }
}

[TestFixture]
[Category("Unit")]
public class CartControllerTests
{
    private Mock<ICartService> _mockCartService = null!;
    private Mock<IProductService> _mockProductService = null!;
    private CartController _controller = null!;

    private static Product MakeProduct(int id = 1, int stock = 10) =>
        new() { Id = id, Name = $"Product {id}", Price = 50m, StockQuantity = stock };

    private static Cart MakeCart() =>
        new() { SessionId = "test", Items = new() };

    [SetUp]
    public void SetUp()
    {
        _mockCartService = new Mock<ICartService>();
        _mockProductService = new Mock<IProductService>();
        _controller = new CartController(_mockCartService.Object, _mockProductService.Object);

        // Setup HttpContext with cookies for all tests
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "ScatterbrainSession=test-session";
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    // ─── GetCart ────────────────────────────────────────────────────────────────

    [Test]
    public async Task GetCart_ReturnsOk_WithCart()
    {
        var cart = MakeCart();
        _mockCartService.Setup(s => s.GetCartAsync(It.IsAny<string>())).ReturnsAsync(cart);

        var result = await _controller.GetCart();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(cart);
    }

    // ─── AddToCart ──────────────────────────────────────────────────────────────

    [Test]
    public async Task AddToCart_ValidRequest_ReturnsOk()
    {
        var product = MakeProduct(1, stock: 5);
        var cart = MakeCart();
        _mockProductService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);
        _mockCartService.Setup(s => s.AddToCartAsync(It.IsAny<string>(), It.IsAny<AddToCartRequest>(), product)).ReturnsAsync(cart);

        var result = await _controller.AddToCart(new AddToCartRequest { ProductId = 1, Quantity = 2 });

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task AddToCart_InvalidQuantity_ReturnsBadRequest()
    {
        var result = await _controller.AddToCart(new AddToCartRequest { ProductId = 1, Quantity = 0 });

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task AddToCart_ProductNotFound_ReturnsNotFound()
    {
        _mockProductService.Setup(s => s.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        var result = await _controller.AddToCart(new AddToCartRequest { ProductId = 999, Quantity = 1 });

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task AddToCart_InsufficientStock_ReturnsBadRequest()
    {
        var product = MakeProduct(1, stock: 2);
        _mockProductService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

        var result = await _controller.AddToCart(new AddToCartRequest { ProductId = 1, Quantity = 10 });

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── UpdateCartItem ─────────────────────────────────────────────────────────

    [Test]
    public async Task UpdateCartItem_ValidRequest_ReturnsOk()
    {
        _mockCartService.Setup(s => s.UpdateCartItemAsync(It.IsAny<string>(), It.IsAny<UpdateCartItemRequest>()))
                        .ReturnsAsync(MakeCart());

        var result = await _controller.UpdateCartItem(new UpdateCartItemRequest { ProductId = 1, Quantity = 3 });

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── RemoveFromCart ─────────────────────────────────────────────────────────

    [Test]
    public async Task RemoveFromCart_ValidId_ReturnsOk()
    {
        _mockCartService.Setup(s => s.RemoveFromCartAsync(It.IsAny<string>(), 1)).ReturnsAsync(MakeCart());

        var result = await _controller.RemoveFromCart(1);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── ClearCart ──────────────────────────────────────────────────────────────

    [Test]
    public async Task ClearCart_ReturnsNoContent()
    {
        _mockCartService.Setup(s => s.ClearCartAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        var result = await _controller.ClearCart();

        result.Should().BeOfType<NoContentResult>();
    }
}
