using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Abstractions.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<int> CountExistingIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
        Task<IEnumerable<Category?>> GetTrackingAsync(List<int> ids, CancellationToken cancellationToken);
        Task<bool> ExistingNameAsync(string name, CancellationToken cancellationToken, int excludeId = 0);
    }
}
