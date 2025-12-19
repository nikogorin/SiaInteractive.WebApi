namespace SiaInteractive.Abstractions.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> InsertAsync(T entity, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<T?> GetAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAllWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<int> Count(CancellationToken cancellationToken);
    }
}
