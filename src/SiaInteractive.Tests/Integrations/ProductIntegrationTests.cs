using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Domain.Entities;
using SiaInteractive.Infraestructure.Persistence;
using SiaInteractive.Tests.Integrations.Factories;

namespace SiaInteractive.Tests.Integrations
{
    [TestClass]
    public class ProductIntegrationTests
    {
        [TestMethod]
        public async Task POST_products_returns_success_and_persists_product()
        {
            // Arrange
            await using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Seeds
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                ctx.Categories.Add(new Category { CategoryID = 1, Name = "Cat1" });
                await ctx.SaveChangesAsync();
            }

            var request = new CreateProductDto
            {
                Name = "Integration Product",
                CategoryIds = [1]
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Product/InsertAsync", request);

            // Assert HTTP
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<Response<bool>>();
            Assert.IsNotNull(body);
            Assert.IsTrue(body!.IsSuccess);
            Assert.IsTrue(body.Data);

            // Assert persistence
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var product = await ctx.Products
                    .Include(p => p.Categories)
                    .FirstOrDefaultAsync(p => p.Name == "Integration Product");

                Assert.IsNotNull(product);
                Assert.AreEqual(1, product!.Categories.Count);
            }
        }
    }
}