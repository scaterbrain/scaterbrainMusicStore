using Microsoft.AspNetCore.Mvc;
using ScatterbrainMusic.API.Models;
using ScatterbrainMusic.API.Services;

namespace ScatterbrainMusic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private const string SessionCookieName = "ScatterbrainSession";

    public CartController(ICartService cartService, IProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
    }

    private string GetOrCreateSessionId()
    {
        if (Request.Cookies.TryGetValue(SessionCookieName, out var sessionId) && !string.IsNullOrEmpty(sessionId))
            return sessionId;

        sessionId = Guid.NewGuid().ToString();
        Response.Cookies.Append(SessionCookieName, sessionId, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        return sessionId;
    }

    /// <summary>
    /// Get current cart
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Cart>> GetCart()
    {
        var sessionId = GetOrCreateSessionId();
        var cart = await _cartService.GetCartAsync(sessionId);
        return Ok(cart);
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("add")]
    public async Task<ActionResult<Cart>> AddToCart([FromBody] AddToCartRequest request)
    {
        if (request.Quantity < 1) return BadRequest("Quantity must be at least 1.");

        var product = await _productService.GetProductByIdAsync(request.ProductId);
        if (product == null) return NotFound($"Product {request.ProductId} not found.");
        if (product.StockQuantity < request.Quantity) return BadRequest("Insufficient stock.");

        var sessionId = GetOrCreateSessionId();
        var cart = await _cartService.AddToCartAsync(sessionId, request, product);
        return Ok(cart);
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    [HttpPut("update")]
    public async Task<ActionResult<Cart>> UpdateCartItem([FromBody] UpdateCartItemRequest request)
    {
        var sessionId = GetOrCreateSessionId();
        var cart = await _cartService.UpdateCartItemAsync(sessionId, request);
        return Ok(cart);
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("{productId:int}")]
    public async Task<ActionResult<Cart>> RemoveFromCart(int productId)
    {
        var sessionId = GetOrCreateSessionId();
        var cart = await _cartService.RemoveFromCartAsync(sessionId, productId);
        return Ok(cart);
    }

    /// <summary>
    /// Clear entire cart
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        var sessionId = GetOrCreateSessionId();
        await _cartService.ClearCartAsync(sessionId);
        return NoContent();
    }
}
