namespace SiaInteractive.WebApi.Modules
{
    /// <summary>
    /// Provides extension methods for configuring CORS policies in the application.
    /// </summary>
    public static class CorsExtensions
    {
        /// <summary>
        /// The name of the CORS policy that allows all origins.
        /// </summary>
        public readonly static string AllowAllOriginsPolicy = "AllowAllOrigins";
        /// <summary>
        /// The name of the CORS policy that restricts origins to a configured list.
        /// </summary>
        public readonly static string RestrictedOriginsPolicy = "RestrictedOrigins";

        /// <summary>
        /// Adds CORS policies to the service collection based on configuration.
        /// </summary>
        /// <param name="services">The service collection to add the CORS policies to.</param>
        /// <param name="configuration">The application configuration containing CORS settings.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOriginsPolicy,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
                options.AddPolicy(RestrictedOriginsPolicy,
                    builder =>
                    {
                        var allowedOrigins = configuration.GetSection("Cors:OriginCors").Get<string[]>();
                        builder.WithOrigins(allowedOrigins ?? Array.Empty<string>())
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            return services;
        }
    }
}
