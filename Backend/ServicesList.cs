using Backend.Services.Auth;
using Backend.Services.Customer;
using Backend.Services.Warehouse;

namespace Backend;

public static class ServicesList
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IWarehouseItemService, WarehouseItemService>();
        services.AddScoped<IWarehouseDeliveryService, WarehouseDeliveryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}
