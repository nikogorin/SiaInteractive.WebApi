using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Abstractions.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetTrackingAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistingNameAsync(string name, CancellationToken cancellationToken, int excludeId = 0);
    }
}