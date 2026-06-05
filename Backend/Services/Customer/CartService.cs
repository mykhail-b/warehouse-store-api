using ClassLibrary.Dto;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Backend.Services.Customer;

/// <summary>
/// Defines operations for managing user shopping carts using distributed cache.
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Retrieves the shopping cart for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of items in the user's cart; empty list if no cart exists.</returns>
    Task<List<CartItemDto>> GetCartAsync(string userId);

    /// <summary>
    /// Adds an item to the user's shopping cart.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="item">The item to add to the cart.</param>
    /// <remarks>If the item already exists in the cart, a duplicate entry is added.</remarks>
    Task AddItemAsync(string userId, CartItemDto item);

    /// <summary>
    /// Removes all items with the specified ID from the user's cart.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="itemId">The ID of the item to remove.</param>
    Task RemoveItemAsync(string userId, int itemId);

    /// <summary>
    /// Clears all items from the user's shopping cart.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    Task ClearCartAsync(string userId);
}

/// <summary>
/// Implementation of <see cref="ICartService"/> using distributed cache for cart storage.
/// Cart data persists for 1 day in the cache before automatic expiration.
/// </summary>
public class CartService : ICartService
{
    private readonly IDistributedCache _cache;

    public CartService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Retrieves the shopping cart for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of cart items; empty list if cache is empty or expired.</returns>
    public async Task<List<CartItemDto>> GetCartAsync(string userId)
    {
        var cached = await _cache.GetStringAsync($"cart:{userId}");
        if (cached == null) return new List<CartItemDto>();
        return JsonSerializer.Deserialize<List<CartItemDto>>(cached)!;
    }

    /// <summary>
    /// Adds an item to the user's shopping cart.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="item">The item to add to the cart.</param>
    public async Task AddItemAsync(string userId, CartItemDto item)
    {
        var cart = await GetCartAsync(userId);
        cart.Add(item);
        await SaveCartAsync(userId, cart);
    }

    /// <summary>
    /// Removes all instances of an item from the user's cart.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="itemId">The ID of the item to remove.</param>
    public async Task RemoveItemAsync(string userId, int itemId)
    {
        var cart = await GetCartAsync(userId);
        cart.RemoveAll(i => i.ItemId == itemId);
        await SaveCartAsync(userId, cart);
    }

    /// <summary>
    /// Clears the entire shopping cart for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    public async Task ClearCartAsync(string userId)
    {
        await _cache.RemoveAsync($"cart:{userId}");
    }

    /// <summary>
    /// Saves the cart to the distributed cache with a 1-day expiration.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cart">The cart items to save.</param>
    private async Task SaveCartAsync(string userId, List<CartItemDto> cart)
    {
        await _cache.SetStringAsync($"cart:{userId}",
            JsonSerializer.Serialize(cart),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
    }
}