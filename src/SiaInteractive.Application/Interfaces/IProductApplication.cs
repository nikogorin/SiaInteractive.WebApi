using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Dtos.Products;

namespace SiaInteractive.Application.Interfaces
{
    public interface IProductApplication
    {
        Task<Response<bool>> InsertAsync(CreateProductDto ProductDto, CancellationToken cancellationToken);
        Task<Response<bool>> UpdateAsync(UpdateProductDto ProductDto, CancellationToken cancellationToken);
        Task<Response<bool>> DeleteAsync(int productId, CancellationToken cancellationToken);
        Task<Response<ProductDto>> GetAsync(int productId, CancellationToken cancellationToken);
        Task<Response<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<ResponsePagination<IEnumerable<ProductDto>>> GetAllWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
