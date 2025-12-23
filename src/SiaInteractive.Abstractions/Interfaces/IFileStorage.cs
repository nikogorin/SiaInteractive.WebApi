namespace SiaInteractive.Abstractions.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveProductImageAsync(Stream contentFile, string originalFileName, CancellationToken cancellationToken);
        Task DeleteIfExistsAsync(string? publicPath, CancellationToken cancellationToken);
    }
}
