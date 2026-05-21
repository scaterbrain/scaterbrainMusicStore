using ScatterbrainMusic.API.Models;

namespace ScatterbrainMusic.API.Services;

public interface IProductService
{
    Task<ProductsResponse> GetProductsAsync(string? category = null, string? search = null, int page = 1, int pageSize = 12);
    Task<Product?> GetProductByIdAsync(int id);
    Task<List<string>> GetCategoriesAsync();
    Task<List<Product>> GetFeaturedProductsAsync();
}

public class ProductService : IProductService
{
    private readonly List<Product> _products = new()
    {
        // Guitars
        new Product { Id = 1, Name = "Fender Stratocaster Player Series", Description = "The iconic Stratocaster shape with modern playability. Perfect for blues, rock, and everything in between.", Price = 849.99m, Category = "Guitars", ImageUrl = "https://images.unsplash.com/photo-1510915361894-db8b60106cb1?w=400&q=80", StockQuantity = 5, IsFeatured = true, Rating = 4.8, ReviewCount = 124, Brand = "Fender", Condition = "New" },
        new Product { Id = 2, Name = "Gibson Les Paul Standard '60s", Description = "Legendary sustain and tone in a classic double-cutaway body. The gold standard for rock guitar.", Price = 2499.99m, Category = "Guitars", ImageUrl = "https://images.unsplash.com/photo-1516924962500-2b4b3b99ea02?w=400&q=80", StockQuantity = 3, IsFeatured = true, Rating = 4.9, ReviewCount = 89, Brand = "Gibson", Condition = "New" },
        new Product { Id = 3, Name = "Martin D-28 Acoustic", Description = "Herringbone trimmed dreadnought with Sitka spruce top. A timeless voice for singer-songwriters.", Price = 3199.99m, Category = "Guitars", ImageUrl = "https://images.unsplash.com/photo-1525201548942-d8732f6617a0?w=400&q=80", StockQuantity = 2, IsFeatured = false, Rating = 4.9, ReviewCount = 67, Brand = "Martin", Condition = "New" },
        new Product { Id = 4, Name = "Vintage 1972 Telecaster", Description = "All-original ash body with maple neck. A true piece of rock history with incredible character.", Price = 4800.00m, Category = "Guitars", ImageUrl = "https://images.unsplash.com/photo-1567016376408-0226e4d0c1ea?w=400&q=80", StockQuantity = 1, IsFeatured = true, Rating = 5.0, ReviewCount = 12, Brand = "Fender", Condition = "Vintage" },
        new Product { Id = 5, Name = "Gretsch G5622T Electromatic", Description = "Semi-hollow centerblock body with Broad'Tron pickups. Rockabilly soul meets modern playability.", Price = 749.99m, Category = "Guitars", ImageUrl = "https://images.unsplash.com/photo-1598653222000-6b7b7a552625?w=400&q=80", StockQuantity = 4, IsFeatured = false, Rating = 4.6, ReviewCount = 43, Brand = "Gretsch", Condition = "New" },

        // Vinyl
        new Product { Id = 6, Name = "Pink Floyd – Dark Side of the Moon", Description = "180g audiophile repress. Side 1 and Side 2 perfectly balanced. The definitive pressing.", Price = 34.99m, Category = "Vinyl", ImageUrl = "https://images.unsplash.com/photo-1603643960248-b9046a5f0f7d?w=400&q=80", StockQuantity = 12, IsFeatured = true, Rating = 5.0, ReviewCount = 203, Brand = "Harvest Records", Condition = "New" },
        new Product { Id = 7, Name = "Miles Davis – Kind of Blue", Description = "Original Columbia 6-eye mono pressing. Near mint condition with original inner sleeve.", Price = 180.00m, Category = "Vinyl", ImageUrl = "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=400&q=80", StockQuantity = 1, IsFeatured = true, Rating = 5.0, ReviewCount = 88, Brand = "Columbia", Condition = "Vintage" },
        new Product { Id = 8, Name = "Fleetwood Mac – Rumours", Description = "2023 remastered 45RPM 2LP set. Crystal clear audio with bonus unreleased material.", Price = 49.99m, Category = "Vinyl", ImageUrl = "https://images.unsplash.com/photo-1471478331149-c72f17e33c73?w=400&q=80", StockQuantity = 8, IsFeatured = false, Rating = 4.9, ReviewCount = 156, Brand = "Reprise Records", Condition = "New" },
        new Product { Id = 9, Name = "Kendrick Lamar – To Pimp a Butterfly", Description = "Double LP on black vinyl. One of the most important albums of the decade.", Price = 39.99m, Category = "Vinyl", ImageUrl = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=400&q=80", StockQuantity = 6, IsFeatured = false, Rating = 4.8, ReviewCount = 97, Brand = "Interscope", Condition = "New" },
        new Product { Id = 10, Name = "Led Zeppelin – IV", Description = "Original UK pressing. All four runes intact. The holy grail of classic rock vinyl.", Price = 220.00m, Category = "Vinyl", ImageUrl = "https://images.unsplash.com/photo-1547394765-185e1e68f34e?w=400&q=80", StockQuantity = 1, IsFeatured = true, Rating = 5.0, ReviewCount = 45, Brand = "Atlantic", Condition = "Vintage" },

        // Gear
        new Product { Id = 11, Name = "Fender Blues Junior IV", Description = "15-watt all-tube combo with spring reverb. The perfect practice and recording amp.", Price = 649.99m, Category = "Gear", ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400&q=80", StockQuantity = 3, IsFeatured = false, Rating = 4.7, ReviewCount = 78, Brand = "Fender", Condition = "New" },
        new Product { Id = 12, Name = "Boss DS-1 Distortion Pedal", Description = "The classic distortion pedal used by everyone from Nirvana to Radiohead. Timeless tone.", Price = 59.99m, Category = "Gear", ImageUrl = "https://images.unsplash.com/photo-1571330735066-03aaa9429d89?w=400&q=80", StockQuantity = 15, IsFeatured = false, Rating = 4.5, ReviewCount = 312, Brand = "Boss", Condition = "New" },
        new Product { Id = 13, Name = "Audio-Technica AT-LP120XBT Turntable", Description = "Direct-drive turntable with Bluetooth and USB. Bring your vinyl collection into the modern era.", Price = 349.99m, Category = "Gear", ImageUrl = "https://images.unsplash.com/photo-1606471191009-63994c53433b?w=400&q=80", StockQuantity = 7, IsFeatured = true, Rating = 4.6, ReviewCount = 184, Brand = "Audio-Technica", Condition = "New" },
        new Product { Id = 14, Name = "Shure SM58 Microphone", Description = "The industry standard vocal microphone. Found on stages from dive bars to arenas worldwide.", Price = 99.99m, Category = "Gear", ImageUrl = "https://images.unsplash.com/photo-1598488035139-bdbb2231ce04?w=400&q=80", StockQuantity = 20, IsFeatured = false, Rating = 4.8, ReviewCount = 445, Brand = "Shure", Condition = "New" },
        new Product { Id = 15, Name = "Roland TD-17KVX Electronic Drum Kit", Description = "Mesh-head electronic kit with module. Practice at any hour without disturbing the neighbours.", Price = 1499.99m, Category = "Gear", ImageUrl = "https://images.unsplash.com/photo-1519892300165-cb5542fb47c7?w=400&q=80", StockQuantity = 2, IsFeatured = true, Rating = 4.7, ReviewCount = 56, Brand = "Roland", Condition = "New" },

        // Accessories
        new Product { Id = 16, Name = "D'Addario EXL110 Guitar Strings (3-Pack)", Description = "Regular light gauge nickel wound strings. The go-to string for electric guitar players worldwide.", Price = 18.99m, Category = "Accessories", ImageUrl = "https://images.unsplash.com/photo-1510915361894-db8b60106cb1?w=400&q=80", StockQuantity = 50, IsFeatured = false, Rating = 4.7, ReviewCount = 892, Brand = "D'Addario", Condition = "New" },
        new Product { Id = 17, Name = "Vinyl Record Cleaning Kit", Description = "Complete care system with carbon fiber brush, cleaning solution, and microfiber cloth.", Price = 29.99m, Category = "Accessories", ImageUrl = "https://images.unsplash.com/photo-1607604276583-eef5d076aa5f?w=400&q=80", StockQuantity = 25, IsFeatured = false, Rating = 4.6, ReviewCount = 134, Brand = "GrooveWasher", Condition = "New" },
        new Product { Id = 18, Name = "Levy's Leather Guitar Strap", Description = "Hand-crafted genuine leather strap with embossed pattern. Comfortable and built to last a lifetime.", Price = 79.99m, Category = "Accessories", ImageUrl = "https://images.unsplash.com/photo-1558098329-a11cff621064?w=400&q=80", StockQuantity = 10, IsFeatured = false, Rating = 4.8, ReviewCount = 67, Brand = "Levy's", Condition = "New" },
    };

    public Task<ProductsResponse> GetProductsAsync(string? category = null, string? search = null, int page = 1, int pageSize = 12)
    {
        var query = _products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category) && category.ToLower() != "all")
            query = query.Where(p => p.Category.ToLower() == category.ToLower());

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Brand.Contains(search, StringComparison.OrdinalIgnoreCase));

        var total = query.Count();
        var products = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new ProductsResponse
        {
            Products = products,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public Task<Product?> GetProductByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<List<string>> GetCategoriesAsync()
    {
        var categories = _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        return Task.FromResult(categories);
    }

    public Task<List<Product>> GetFeaturedProductsAsync()
    {
        var featured = _products.Where(p => p.IsFeatured).Take(6).ToList();
        return Task.FromResult(featured);
    }
}
