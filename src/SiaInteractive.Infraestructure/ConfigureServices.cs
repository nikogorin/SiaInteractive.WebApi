using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiaInteractive.Abstractions.Interfaces;
using SiaInteractive.Infraestructure.Repositories;
using SiaInteractive.Infraestructure.Storage;

namespace SiaInteractive.Infraestructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddIntraestructureServices(this IServiceCollection services, IConfiguration configuration, string webRootPath)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IFileStorage, FileStorage>();
            services.Configure<FileStorageOptions>(options => 
            {
                options.RootPath = webRootPath!;
                options.UploadFolder = "uploads";
            });
            return services;
        }
    }
}
