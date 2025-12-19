using Microsoft.EntityFrameworkCore;
using SiaInteractive.Abstractions.Interfaces;
using SiaInteractive.Domain.Entities;
using SiaInteractive.Infraestructure.Persistence;

namespace SiaInteractive.Infraestructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Count(CancellationToken cancellationToken)
        {
            return await _context.Categories.CountAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id, cancellationToken);

            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetAllWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return Enumerable.Empty<Category>();

            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == id, cancellationToken);
        }

        public async Task<IEnumerable<Category?>> GetTrackingAsync(List<int> ids, CancellationToken cancellationToken)
        {
            var categories = await _context.Categories.Where(c => ids.Contains(c.CategoryID)).ToListAsync(cancellationToken);
            return categories;
        }

        public async Task<bool> InsertAsync(Category entity, CancellationToken cancellationToken)
        {
            await _context.Categories.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateAsync(Category entity, CancellationToken cancellationToken)
        {
            var eCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == entity.CategoryID, cancellationToken);

            if (eCategory == null)
                return false;

            eCategory.Name = entity.Name;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<int> CountExistingIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            return await _context.Categories.CountAsync(c => ids.Contains(c.CategoryID), cancellationToken);
        }

        public async Task<bool> ExistingNameAsync(string name, CancellationToken cancellationToken, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            var normalizedName = name.Trim();
            return await _context.Categories.AnyAsync(c => c.CategoryID != excludeId && c.Name == normalizedName, cancellationToken);
        }
    }
}
