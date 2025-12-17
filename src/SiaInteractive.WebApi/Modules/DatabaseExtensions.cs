using Microsoft.EntityFrameworkCore;
using SiaInteractive.Infraestructure.Persistence;

namespace SiaInteractive.WebApi.Modules
{
    /// <summary>
    /// Provides extension methods for configuring the db context in the service collection.
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Adds and configures the db context generation services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add the DbContext services to.</param>
        /// <param name="configuration"></param>
        /// <returns>The same service collection instance so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
