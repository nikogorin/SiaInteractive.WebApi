using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Infraestructure.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<int> CountExistingIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Category?>> GetTrackingAsync(List<int> ids);
        Task<bool> ExistingNameAsync(string name);
    }
}
