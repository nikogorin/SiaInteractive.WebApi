using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SiaInteractive.Application.Core;
using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Application.Interfaces;
using SiaInteractive.Application.Validators.Categories;
using SiaInteractive.Application.Validators.Products;
using System.Reflection;

namespace SiaInteractive.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Application Services Registration
            services.AddScoped<ICategoryApplication, CategoryApplication>();
            services.AddScoped<IProductApplication, ProductApplication>();

            // Validators Registration
            services.AddTransient<IValidator<CreateCategoryDto>, CreateCategoryDtoValidator>();
            services.AddTransient<IValidator<UpdateCategoryDto>, UpdateCategoryDtoValidator>();
            services.AddTransient<IValidator<CreateProductDto>, CreateProductDtoValidator>();
            services.AddTransient<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();

            // AutoMapper Configuration
            services.AddAutoMapper(cfg => { }, assemblies: Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
