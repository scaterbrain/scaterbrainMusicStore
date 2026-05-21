using FluentAssertions;
using NUnit.Framework;
using ScatterbrainMusic.API.Models;
using ScatterbrainMusic.API.Services;

namespace ScatterbrainMusic.Tests.Services;

[TestFixture]
[Category("Unit")]
public class CartServiceTests
{
    private CartService _service = null!;
    private const string TestSessionId = "test-session-abc123";

    private static Product MakeProduct(int id = 1, decimal price = 99.99m, int stock = 10) =>
        new() { Id = id, Name = $"Product {id}", Price = price, StockQuantity = stock, ImageUrl = "https://example.com/img.jpg" };

    [SetUp]
    public void SetUp()
    {
        _service = new CartService();
    }

    // ─── GetCartAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task GetCartAsync_NewSession_ReturnsEmptyCart()
    {
        var cart = await _service.GetCartAsync(TestSessionId);

        cart.Should().NotBeNull();
        cart.SessionId.Should().Be(TestSessionId);
        cart.Items.Should().BeEmpty();
        cart.Total.Should().Be(0);
        cart.ItemCount.Should().Be(0);
    }

    [Test]
    public async Task GetCartAsync_SameSession_ReturnsSameCart()
    {
        var cart1 = await _service.GetCartAsync(TestSessionId);
        var cart2 = await _service.GetCartAsync(TestSessionId);

        cart1.Should().BeSameAs(cart2);
    }

    // ─── AddToCartAsync ─────────────────────────────────────────────────────────

    [Test]
    public async Task AddToCartAsync_NewProduct_AddsItemToCart()
    {
        var product = MakeProduct(1, 49.99m);
        var request = new AddToCartRequest { ProductId = 1, Quantity = 2 };

        var cart = await _service.AddToCartAsync(TestSessionId, request, product);

        cart.Items.Should().HaveCount(1);
        cart.Items[0].ProductId.Should().Be(1);
        cart.Items[0].Quantity.Should().Be(2);
        cart.Items[0].Price.Should().Be(49.99m);
    }

    [Test]
    public async Task AddToCartAsync_ExistingProduct_IncrementsQuantity()
    {
        var product = MakeProduct(1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 1 }, product);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 3 }, product);

        var cart = await _service.GetCartAsync(TestSessionId);

        cart.Items.Should().HaveCount(1);
        cart.Items[0].Quantity.Should().Be(4);
    }

    [Test]
    public async Task AddToCartAsync_MultipleProducts_AddsAll()
    {
        var p1 = MakeProduct(1, 100m);
        var p2 = MakeProduct(2, 200m);

        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 1 }, p1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 2, Quantity = 1 }, p2);

        var cart = await _service.GetCartAsync(TestSessionId);

        cart.Items.Should().HaveCount(2);
    }

    [Test]
    public async Task AddToCartAsync_TotalCalculatedCorrectly()
    {
        var p1 = MakeProduct(1, 10.00m);
        var p2 = MakeProduct(2, 20.00m);

        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 3 }, p1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 2, Quantity = 2 }, p2);

        var cart = await _service.GetCartAsync(TestSessionId);

        // 3 × $10 + 2 × $20 = $70
        cart.Total.Should().Be(70.00m);
        cart.ItemCount.Should().Be(5);
    }

    [Test]
    public async Task AddToCartAsync_NewSession_CreatesCartAutomatically()
    {
        var product = MakeProduct(1);
        var cart = await _service.AddToCartAsync("brand-new-session", new AddToCartRequest { ProductId = 1, Quantity = 1 }, product);

        cart.Should().NotBeNull();
        cart.Items.Should().HaveCount(1);
    }

    // ─── UpdateCartItemAsync ────────────────────────────────────────────────────

    [Test]
    public async Task UpdateCartItemAsync_ValidProduct_UpdatesQuantity()
    {
        var product = MakeProduct(1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 1 }, product);

        var cart = await _service.UpdateCartItemAsync(TestSessionId, new UpdateCartItemRequest { ProductId = 1, Quantity = 5 });

        cart.Items[0].Quantity.Should().Be(5);
    }

    [Test]
    public async Task UpdateCartItemAsync_QuantityZero_RemovesItem()
    {
        var product = MakeProduct(1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 2 }, product);

        var cart = await _service.UpdateCartItemAsync(TestSessionId, new UpdateCartItemRequest { ProductId = 1, Quantity = 0 });

        cart.Items.Should().BeEmpty();
    }

    [Test]
    public async Task UpdateCartItemAsync_NegativeQuantity_RemovesItem()
    {
        var product = MakeProduct(1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 1 }, product);

        var cart = await _service.UpdateCartItemAsync(TestSessionId, new UpdateCartItemRequest { ProductId = 1, Quantity = -1 });

        cart.Items.Should().BeEmpty();
    }

    [Test]
    public async Task UpdateCartItemAsync_NonExistentProduct_DoesNotThrow()
    {
        Func<Task> act = async () =>
            await _service.UpdateCartItemAsync(TestSessionId, new UpdateCartItemRequest { ProductId = 999, Quantity = 1 });

        await act.Should().NotThrowAsync();
    }

    // ─── RemoveFromCartAsync ────────────────────────────────────────────────────

    [Test]
    public async Task RemoveFromCartAsync_ExistingItem_RemovesIt()
    {
        var product = MakeProduct(1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 2 }, product);

        var cart = await _service.RemoveFromCartAsync(TestSessionId, 1);

        cart.Items.Should().BeEmpty();
    }

    [Test]
    public async Task RemoveFromCartAsync_RemovesOnlySpecifiedItem()
    {
        var p1 = MakeProduct(1);
        var p2 = MakeProduct(2);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 1 }, p1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 2, Quantity = 1 }, p2);

        var cart = await _service.RemoveFromCartAsync(TestSessionId, 1);

        cart.Items.Should().HaveCount(1);
        cart.Items[0].ProductId.Should().Be(2);
    }

    [Test]
    public async Task RemoveFromCartAsync_NonExistentItem_DoesNotThrow()
    {
        Func<Task> act = async () => await _service.RemoveFromCartAsync(TestSessionId, 999);

        await act.Should().NotThrowAsync();
    }

    // ─── ClearCartAsync ─────────────────────────────────────────────────────────

    [Test]
    public async Task ClearCartAsync_CartWithItems_RemovesAll()
    {
        var p1 = MakeProduct(1);
        var p2 = MakeProduct(2);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 1, Quantity = 1 }, p1);
        await _service.AddToCartAsync(TestSessionId, new AddToCartRequest { ProductId = 2, Quantity = 2 }, p2);

        await _service.ClearCartAsync(TestSessionId);
        var cart = await _service.GetCartAsync(TestSessionId);

        cart.Items.Should().BeEmpty();
        cart.Total.Should().Be(0);
    }

    [Test]
    public async Task ClearCartAsync_EmptyCart_DoesNotThrow()
    {
        Func<Task> act = async () => await _service.ClearCartAsync(TestSessionId);

        await act.Should().NotThrowAsync();
    }
}
