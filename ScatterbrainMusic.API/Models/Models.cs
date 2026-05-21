namespace ScatterbrainMusic.API.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsFeatured { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Condition { get; set; } = "New"; // New, Used, Vintage
}

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class Cart
{
    public string SessionId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    public int ItemCount => Items.Sum(i => i.Quantity);
}

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class ProductsResponse
{
    public List<Product> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
