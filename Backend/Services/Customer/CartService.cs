using ClassLibrary.Dto;
using ClassLibrary.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Backend.Services.Customer;


public interface ICartService
{
    Task<List<CartItemDto>> GetCartAsync();
    Task AddItemAsync(CartItemDto item);
    Task RemoveItemAsync(int itemId);
    Task ClearCartAsync();
}

public class CartService : ICartService
{
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CookieSettings _cookieSettings;
    private const string CartCookieName = "Warehouse_Cart_Id";

    public CartService(
        IDistributedCache cache,
        IHttpContextAccessor httpContextAccessor,
        IOptions<CookieSettings> cookieSettingsOptions)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _cookieSettings = cookieSettingsOptions.Value;
    }

    public async Task<List<CartItemDto>> GetCartAsync()
    {
        var cartId = GetOrCreateCartId();
        var cached = await _cache.GetStringAsync($"cart:{cartId}");

        if (cached == null) return new List<CartItemDto>();
        return JsonSerializer.Deserialize<List<CartItemDto>>(cached)!;
    }

    public async Task AddItemAsync(CartItemDto item)
    {
        var cart = await GetCartAsync();
        cart.Add(item);
        await SaveCartAsync(cart);
    }

    public async Task RemoveItemAsync(int itemId)
    {
        var cart = await GetCartAsync();
        cart.RemoveAll(i => i.ItemId == itemId);
        await SaveCartAsync(cart);
    }

    public async Task ClearCartAsync()
    {
        var cartId = GetOrCreateCartId();
        await _cache.RemoveAsync($"cart:{cartId}");

        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CartCookieName);
    }

    // Internal method of storing data in Redis
    private async Task SaveCartAsync(List<CartItemDto> cart)
    {
        var cartId = GetOrCreateCartId();

        await _cache.SetStringAsync($"cart:{cartId}",
            JsonSerializer.Serialize(cart),
            new DistributedCacheEntryOptions
            {
                // The lifetime of the Redis bucket is taken from our general appsettings settings.
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_cookieSettings.DefaultExpirationDays)
            });
    }
    private string GetOrCreateCartId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) throw new InvalidOperationException("HTTP context is unavailable.");

        // Checking if the browser has sent the shopping cart cookie
        if (context.Request.Cookies.TryGetValue(CartCookieName, out var cartId) && !string.IsNullOrEmpty(cartId))
        {
            return cartId;
        }

        // If there is no cookie, we create a new unique cart token.
        var newCartId = Guid.NewGuid().ToString();

        // Convert SameSite from a settings string to an Enum
        var sameSiteMode = _cookieSettings.SameSite switch
        {
            "Strict" => SameSiteMode.Strict,
            "Lax" => SameSiteMode.Lax,
            _ => SameSiteMode.None
        };

        // Implementing security options based on appsettings.json.
        var cookieOptions = new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            Expires = DateTime.UtcNow.AddDays(_cookieSettings.DefaultExpirationDays),
            SameSite = sameSiteMode
        };

        // Embed a cookie into the browser's response
        context.Response.Cookies.Append(CartCookieName, newCartId, cookieOptions);

        return newCartId;
    }
}