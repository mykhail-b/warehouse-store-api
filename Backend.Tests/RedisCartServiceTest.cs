//using Testcontainers.Redis;
//using Microsoft.Extensions.Caching.StackExchangeRedis;
//using ClassLibrary.Dto;
//using Backend.Services.Customer;

//namespace Backend.Tests;

//public class RedisCartServiceTest : IAsyncLifetime
//{
//    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
//    private ICartService _cartService = null!;
        
//    [Fact]
//    public async Task AddItem_ShouldBeInCart()
//    {
//        var item = new CartItemDto { ItemId = 1, Name = "Test", Quantity = 2, Price = 10 };

//        await _cartService.AddItemAsync("user1", item);
//        var cart = await _cartService.GetCartAsync("user1");

//        Assert.Single(cart);
//        Assert.Equal(1, cart[0].ItemId);
//    }

//    async ValueTask IAsyncLifetime.InitializeAsync()
//    {
//        await _redisContainer.StartAsync();

//        var cache = new RedisCache(new RedisCacheOptions
//        {
//            Configuration = _redisContainer.GetConnectionString()
//        });

//        _cartService = new CartService(cache);
//    }

//    async ValueTask IAsyncDisposable.DisposeAsync()
//        => await _redisContainer.DisposeAsync();
//}