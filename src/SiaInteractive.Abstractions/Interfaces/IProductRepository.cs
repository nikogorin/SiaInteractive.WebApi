using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Infraestructure.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetTrackingAsync(int id);
        Task<bool> ExistingNameAsync(string name);
    }
}
