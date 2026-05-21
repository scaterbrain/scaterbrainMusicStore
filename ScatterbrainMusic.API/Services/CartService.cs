using ScatterbrainMusic.API.Models;

namespace ScatterbrainMusic.API.Services;

public interface ICartService
{
    Task<Cart> GetCartAsync(string sessionId);
    Task<Cart> AddToCartAsync(string sessionId, AddToCartRequest request, Product product);
    Task<Cart> UpdateCartItemAsync(string sessionId, UpdateCartItemRequest request);
    Task<Cart> RemoveFromCartAsync(string sessionId, int productId);
    Task ClearCartAsync(string sessionId);
}

public class CartService : ICartService
{
    private readonly Dictionary<string, Cart> _carts = new();

    public Task<Cart> GetCartAsync(string sessionId)
    {
        if (!_carts.TryGetValue(sessionId, out var cart))
        {
            cart = new Cart { SessionId = sessionId };
            _carts[sessionId] = cart;
        }
        return Task.FromResult(cart);
    }

    public Task<Cart> AddToCartAsync(string sessionId, AddToCartRequest request, Product product)
    {
        if (!_carts.TryGetValue(sessionId, out var cart))
        {
            cart = new Cart { SessionId = sessionId };
            _carts[sessionId] = cart;
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = request.Quantity,
                ImageUrl = product.ImageUrl
            });
        }

        return Task.FromResult(cart);
    }

    public Task<Cart> UpdateCartItemAsync(string sessionId, UpdateCartItemRequest request)
    {
        if (!_carts.TryGetValue(sessionId, out var cart))
        {
            cart = new Cart { SessionId = sessionId };
            _carts[sessionId] = cart;
        }

        var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (item != null)
        {
            if (request.Quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = request.Quantity;
        }

        return Task.FromResult(cart);
    }

    public Task<Cart> RemoveFromCartAsync(string sessionId, int productId)
    {
        if (_carts.TryGetValue(sessionId, out var cart))
        {
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
                cart.Items.Remove(item);
        }
        else
        {
            cart = new Cart { SessionId = sessionId };
        }
        return Task.FromResult(cart);
    }

    public Task ClearCartAsync(string sessionId)
    {
        if (_carts.TryGetValue(sessionId, out var cart))
            cart.Items.Clear();
        return Task.CompletedTask;
    }
}
