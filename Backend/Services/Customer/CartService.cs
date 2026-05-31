using ClassLibrary.Dto;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Backend.Services.Customer;

public interface ICartService
{
    Task<List<CartItemDto>> GetCartAsync(string userId);
    Task AddItemAsync(string userId, CartItemDto item);
    Task RemoveItemAsync(string userId, long itemId);
    Task ClearCartAsync(string userId);
}

public class CartService : ICartService
{
    private readonly IDistributedCache _cache;

    public CartService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<List<CartItemDto>> GetCartAsync(string userId)
    {
        var cached = await _cache.GetStringAsync($"cart:{userId}");
        if (cached == null) return new List<CartItemDto>();
        return JsonSerializer.Deserialize<List<CartItemDto>>(cached)!;
    }

    public async Task AddItemAsync(string userId, CartItemDto item)
    {
        var cart = await GetCartAsync(userId);
        cart.Add(item);
        await SaveCartAsync(userId, cart);
    }

    public async Task RemoveItemAsync(string userId, long itemId)
    {
        var cart = await GetCartAsync(userId);
        cart.RemoveAll(i => i.ItemId == itemId);
        await SaveCartAsync(userId, cart);
    }

    public async Task ClearCartAsync(string userId)
    {
        await _cache.RemoveAsync($"cart:{userId}");
    }

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