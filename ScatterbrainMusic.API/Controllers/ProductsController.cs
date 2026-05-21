using Microsoft.AspNetCore.Mvc;
using ScatterbrainMusic.API.Models;
using ScatterbrainMusic.API.Services;

namespace ScatterbrainMusic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ProductsResponse>> GetProducts(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 12;

        var result = await _productService.GetProductsAsync(category, search, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get a single product by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    /// <summary>
    /// Get all available categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetCategories()
    {
        var categories = await _productService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get featured products for homepage
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<List<Product>>> GetFeatured()
    {
        var products = await _productService.GetFeaturedProductsAsync();
        return Ok(products);
    }
}
