namespace StoreApi.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStoreScopedServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }
    }
}