using ERP.Repositories;
using ERP.Services;

namespace ERP.Extensions
{
    public static class ServiceClassExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Register custom services
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IProductOrderService, ProdcutOrderService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();


            // Add more services as needed
            return services;
        }
    }
}
