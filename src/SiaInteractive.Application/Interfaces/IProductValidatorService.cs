using SiaInteractive.Application.Dtos.Products;

namespace SiaInteractive.Application.Interfaces
{
    public interface IProductValidatorService
    {
        Task<bool> ProductMustExist(int productId, CancellationToken cancellationToken);
        Task<bool> NameMustBeUnique(string name, CancellationToken cancellationToken);
        Task<bool> NameMustBeUnique(string name, UpdateProductDto dto, CancellationToken cancellationToken);
    }
}
