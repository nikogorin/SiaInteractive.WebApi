using Microsoft.OpenApi;

namespace SiaInteractive.WebApi.Modules
{
    /// <summary>
    /// Provides extension methods for configuring Swagger in the service collection.
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds and configures Swagger generation services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add Swagger services to.</param>
        /// <returns>The same service collection instance so that multiple calls can be chained.</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sia Interactive WebApi",
                    Version = "v1",
                    Description = "API documentation for Sia Interactive WebApi",
                    Contact = new OpenApiContact
                    {
                        Name = "Sia contact",
                        Email = "nikogorin@hotmail.com",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.EnableAnnotations();
            });

            return services;
        }
    }
}
