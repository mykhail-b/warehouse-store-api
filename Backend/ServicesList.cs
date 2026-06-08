using Backend.Services.Auth;
using Backend.Services.Customer;
using Backend.Services.Infrastructure;
using Backend.Services.Warehouse;

namespace Backend;

public static class ServicesList
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IMailService, MailService>();

        return services;
    }
}
