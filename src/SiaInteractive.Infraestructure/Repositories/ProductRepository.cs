using Microsoft.EntityFrameworkCore;
using SiaInteractive.Abstractions.Interfaces;
using SiaInteractive.Domain.Entities;
using SiaInteractive.Infraestructure.Persistence;

namespace SiaInteractive.Infraestructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Count(CancellationToken cancellationToken)
        {
            return await _context.Products.CountAsync();
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return Enumerable.Empty<Product>();

            return await _context.Products
                .Include(p => p.Categories)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductID == id, cancellationToken);
        }

        public async Task<Product?> GetTrackingAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.ProductID == id, cancellationToken);
        }

        public async Task<bool> InsertAsync(Product entity, CancellationToken cancellationToken)
        {
            await _context.Products.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateAsync(Product entity, CancellationToken cancellationToken)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistingNameAsync(string name, CancellationToken cancellationToken, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            var normalizedName = name.Trim();
            return await _context.Products.AnyAsync(p => p.ProductID != excludeId && p.Name == normalizedName, cancellationToken);
        }
    }
}
