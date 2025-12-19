using Microsoft.EntityFrameworkCore;
using SiaInteractive.Domain.Entities;
using SiaInteractive.Infraestructure.Persistence;
using SiaInteractive.Infraestructure.Repositories;

namespace SiaInteractive.Tests.Repositories;

[TestClass]
public class ProductRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(options);
    }

    private static async Task SeedAsync(AppDbContext dbContext)
    {
        var cat1 = new Category { CategoryID = 1, Name = "Cat1" };
        var cat2 = new Category { CategoryID = 2, Name = "Cat2" };
        dbContext.Categories.AddRange(cat1, cat2);

        var p1 = new Product { ProductID = 1, Name = "Zeta", Categories = new List<Category> { cat1 } };
        var p2 = new Product { ProductID = 2, Name = "Alpha", Categories = new List<Category> { cat1, cat2 } };
        var p3 = new Product { ProductID = 3, Name = "Beta", Categories = new List<Category>() };

        dbContext.Products.AddRange(p1, p2, p3);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }


    [TestMethod]
    public async Task Count_ReturnsNumberOfProducts()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var count = await repo.Count(ct);

        Assert.AreEqual(3, count);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenProductExists_RemovesAndReturnsTrue()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var ok = await repo.DeleteAsync(2, ct);

        Assert.IsTrue(ok);
        Assert.AreEqual(2, await repo.Count(ct));
        Assert.IsNull(await dbContext.Products.FindAsync(2));
    }

    [TestMethod]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var ok = await repo.DeleteAsync(999, ct);

        Assert.IsFalse(ok);
        Assert.AreEqual(3, await repo.Count(ct));
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsOrderedByName_AsNoTracking()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var result = (await repo.GetAllAsync(ct)).ToList();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Alpha", result[0].Name);
        Assert.AreEqual("Beta", result[1].Name);
        Assert.AreEqual("Zeta", result[2].Name);
        Assert.AreEqual(0, dbContext.ChangeTracker.Entries().Count());
    }

    [TestMethod]
    public async Task GetAllWithPaginationAsync_WhenInvalidPageOrSize_ReturnsEmpty()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var r1 = await repo.GetAllWithPaginationAsync(0, 10, ct);
        var r2 = await repo.GetAllWithPaginationAsync(1, 0, ct);

        Assert.AreEqual(0, r1.Count());
        Assert.AreEqual(0, r2.Count());
    }

    [TestMethod]
    public async Task GetAllWithPaginationAsync_ReturnsCorrectSlice_OrderedByName_AndIncludesCategories()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var page = (await repo.GetAllWithPaginationAsync(2, 1, ct)).ToList();

        Assert.AreEqual(1, page.Count);
        Assert.AreEqual("Beta", page[0].Name);
        Assert.IsNotNull(page[0].Categories);
    }

    [TestMethod]
    public async Task GetAsync_ReturnsProductWithCategories_AsNoTracking()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var product = await repo.GetAsync(2, ct);

        Assert.IsNotNull(product);
        Assert.AreEqual(2, product!.ProductID);
        Assert.IsNotNull(product.Categories);
        Assert.AreEqual(2, product.Categories.Count);
        Assert.AreEqual(0, dbContext.ChangeTracker.Entries().Count());
    }

    [TestMethod]
    public async Task GetTrackingAsync_ReturnsTrackedEntity_WithCategories()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var product = await repo.GetTrackingAsync(2, ct);

        Assert.IsNotNull(product);
        Assert.AreEqual("Alpha", product!.Name);
        Assert.IsNotNull(product.Categories);
        Assert.AreEqual(2, product.Categories.Count);
        Assert.IsTrue(dbContext.ChangeTracker.Entries<Product>().Any());
    }

    [TestMethod]
    public async Task InsertAsync_AddsProductAndReturnsTrue()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);

        var repo = new ProductRepository(dbContext);

        var entity = new Product
        {
            ProductID = 10,
            Name = "New",
            Categories = new List<Category>()
        };

        var ok = await repo.InsertAsync(entity, ct);

        Assert.IsTrue(ok);
        Assert.AreEqual(1, await repo.Count(ct));
        Assert.IsNotNull(await dbContext.Products.FindAsync(10));
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesProductAndReturnsTrue()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var tracked = await dbContext.Products.Include(p => p.Categories)
                                        .FirstAsync(p => p.ProductID == 1);

        tracked.Name = "A-Updated";

        var ok = await repo.UpdateAsync(tracked, ct);

        Assert.IsTrue(ok);

        dbContext.ChangeTracker.Clear();
        var updated = await dbContext.Products.FindAsync(1);
        Assert.AreEqual("A-Updated", updated!.Name);
    }

    [TestMethod]
    public async Task ExistingNameAsync_WhenNameIsNullOrWhitespace_ReturnsTrue()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        Assert.IsTrue(await repo.ExistingNameAsync(null!, ct));
        Assert.IsTrue(await repo.ExistingNameAsync("", ct));
        Assert.IsTrue(await repo.ExistingNameAsync("   ", ct));
    }

    [TestMethod]
    public async Task ExistingNameAsync_WhenNameExists_ReturnsTrue_Trimmed()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var exists = await repo.ExistingNameAsync("  Alpha  ", ct);

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public async Task ExistingNameAsync_WhenNameDoesNotExist_ReturnsFalse()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var exists = await repo.ExistingNameAsync("DoesNotExist", ct);

        Assert.IsFalse(exists);
    }

    [TestMethod]
    public async Task ExistingNameAsync_ExcludeId_IgnoresThatSameEntity()
    {
        var ct = CancellationToken.None;
        var dbName = Guid.NewGuid().ToString();
        using var dbContext = CreateContext(dbName);
        await SeedAsync(dbContext);

        var repo = new ProductRepository(dbContext);

        var exists = await repo.ExistingNameAsync("Alpha", ct, excludeId: 2);

        Assert.IsFalse(exists);
    }
}
