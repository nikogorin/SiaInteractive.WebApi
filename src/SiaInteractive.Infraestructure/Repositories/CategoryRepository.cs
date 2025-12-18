using Microsoft.EntityFrameworkCore;
using SiaInteractive.Domain.Entities;
using SiaInteractive.Infraestructure.Interfaces;
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

        public async Task<int> Count()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllWithPaginationAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return Enumerable.Empty<Category>();

            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Category?> GetAsync(int id)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == id);
        }

        public async Task<IEnumerable<Category?>> GetTrackingAsync(List<int> ids)
        {
            var categories = await _context.Categories.Where(c => ids.Contains(c.CategoryID)).ToListAsync();
            return categories;
        }

        public async Task<bool> InsertAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Category entity)
        {
            var eCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == entity.CategoryID);

            if (eCategory == null)
                return false;

            eCategory.Name = entity.Name;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountExistingIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Categories.CountAsync(c => ids.Contains(c.CategoryID));
        }

        public async Task<bool> ExistingNameAsync(string name, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            var normalizedName = name.Trim();
            return await _context.Categories.AnyAsync(c => c.CategoryID != excludeId && c.Name == normalizedName);
        }
    }
}
