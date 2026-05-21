using FluentAssertions;
using NUnit.Framework;
using ScatterbrainMusic.API.Services;

namespace ScatterbrainMusic.Tests.Services;

[TestFixture]
[Category("Unit")]
public class ProductServiceTests
{
    private ProductService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new ProductService();
    }

    // ─── GetProductsAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task GetProductsAsync_NoFilter_ReturnsAllProducts()
    {
        var result = await _service.GetProductsAsync();

        result.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThan(0);
        result.Products.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetProductsAsync_FilterByCategory_ReturnsOnlyMatchingProducts()
    {
        var result = await _service.GetProductsAsync(category: "Guitars");

        result.Products.Should().AllSatisfy(p =>
            p.Category.Should().Be("Guitars"));
    }

    [Test]
    [TestCase("guitars")]
    [TestCase("GUITARS")]
    [TestCase("Guitars")]
    public async Task GetProductsAsync_CategoryFilter_IsCaseInsensitive(string category)
    {
        var result = await _service.GetProductsAsync(category: category);

        result.Products.Should().NotBeEmpty();
        result.Products.Should().AllSatisfy(p =>
            p.Category.ToLower().Should().Be("guitars"));
    }

    [Test]
    public async Task GetProductsAsync_FilterBySearch_MatchesNameDescriptionOrBrand()
    {
        var result = await _service.GetProductsAsync(search: "Fender");

        result.Products.Should().NotBeEmpty();
        result.Products.Should().AllSatisfy(p =>
            (p.Name.Contains("Fender", StringComparison.OrdinalIgnoreCase) ||
             p.Description.Contains("Fender", StringComparison.OrdinalIgnoreCase) ||
             p.Brand.Contains("Fender", StringComparison.OrdinalIgnoreCase)).Should().BeTrue());
    }

    [Test]
    public async Task GetProductsAsync_SearchNoResults_ReturnsEmptyList()
    {
        var result = await _service.GetProductsAsync(search: "xyzzy_does_not_exist_12345");

        result.Products.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Test]
    public async Task GetProductsAsync_Pagination_ReturnsCorrectPage()
    {
        var page1 = await _service.GetProductsAsync(page: 1, pageSize: 5);
        var page2 = await _service.GetProductsAsync(page: 2, pageSize: 5);

        page1.Products.Should().HaveCount(5);
        page1.Products.Select(p => p.Id).Should().NotIntersectWith(page2.Products.Select(p => p.Id));
    }

    [Test]
    public async Task GetProductsAsync_PageSizeRespected()
    {
        var result = await _service.GetProductsAsync(pageSize: 3);

        result.Products.Should().HaveCount(3);
    }

    [Test]
    public async Task GetProductsAsync_CategoryAndSearch_BothFiltersApplied()
    {
        var result = await _service.GetProductsAsync(category: "Guitars", search: "Fender");

        result.Products.Should().NotBeEmpty();
        result.Products.Should().AllSatisfy(p =>
        {
            p.Category.Should().Be("Guitars");
            (p.Name.Contains("Fender", StringComparison.OrdinalIgnoreCase) ||
             p.Brand.Contains("Fender", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        });
    }

    [Test]
    public async Task GetProductsAsync_AllCategoryFilter_ReturnsAllProducts()
    {
        var allResult = await _service.GetProductsAsync(pageSize: 100);
        var allCategoryResult = await _service.GetProductsAsync(category: "all", pageSize: 100);

        allCategoryResult.TotalCount.Should().Be(allResult.TotalCount);
    }

    // ─── GetProductByIdAsync ────────────────────────────────────────────────────

    [Test]
    public async Task GetProductByIdAsync_ValidId_ReturnsProduct()
    {
        var product = await _service.GetProductByIdAsync(1);

        product.Should().NotBeNull();
        product!.Id.Should().Be(1);
    }

    [Test]
    public async Task GetProductByIdAsync_InvalidId_ReturnsNull()
    {
        var product = await _service.GetProductByIdAsync(999999);

        product.Should().BeNull();
    }

    [Test]
    public async Task GetProductByIdAsync_ProductHasRequiredFields()
    {
        var product = await _service.GetProductByIdAsync(1);

        product.Should().NotBeNull();
        product!.Name.Should().NotBeNullOrWhiteSpace();
        product.Description.Should().NotBeNullOrWhiteSpace();
        product.Price.Should().BeGreaterThan(0);
        product.Category.Should().NotBeNullOrWhiteSpace();
        product.Brand.Should().NotBeNullOrWhiteSpace();
    }

    // ─── GetCategoriesAsync ─────────────────────────────────────────────────────

    [Test]
    public async Task GetCategoriesAsync_ReturnsDistinctCategories()
    {
        var categories = await _service.GetCategoriesAsync();

        categories.Should().NotBeEmpty();
        categories.Should().OnlyHaveUniqueItems();
    }

    [Test]
    public async Task GetCategoriesAsync_ReturnsExpectedCategories()
    {
        var categories = await _service.GetCategoriesAsync();

        categories.Should().Contain("Guitars");
        categories.Should().Contain("Vinyl");
        categories.Should().Contain("Gear");
        categories.Should().Contain("Accessories");
    }

    [Test]
    public async Task GetCategoriesAsync_ReturnsSortedAlphabetically()
    {
        var categories = await _service.GetCategoriesAsync();

        categories.Should().BeInAscendingOrder();
    }

    // ─── GetFeaturedProductsAsync ───────────────────────────────────────────────

    [Test]
    public async Task GetFeaturedProductsAsync_ReturnsOnlyFeaturedProducts()
    {
        var featured = await _service.GetFeaturedProductsAsync();

        featured.Should().NotBeEmpty();
        featured.Should().AllSatisfy(p => p.IsFeatured.Should().BeTrue());
    }

    [Test]
    public async Task GetFeaturedProductsAsync_ReturnsAtMostSixProducts()
    {
        var featured = await _service.GetFeaturedProductsAsync();

        featured.Should().HaveCountLessThanOrEqualTo(6);
    }

    [Test]
    public async Task GetFeaturedProductsAsync_ProductsHaveValidData()
    {
        var featured = await _service.GetFeaturedProductsAsync();

        featured.Should().AllSatisfy(p =>
        {
            p.Id.Should().BeGreaterThan(0);
            p.Name.Should().NotBeNullOrWhiteSpace();
            p.Price.Should().BeGreaterThan(0);
        });
    }
}
