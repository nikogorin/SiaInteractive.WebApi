using Microsoft.Extensions.DependencyInjection;
using SiaInteractive.Infraestructure.Interfaces;
using SiaInteractive.Infraestructure.Repositories;

namespace SiaInteractive.Infraestructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddIntraestructureServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
