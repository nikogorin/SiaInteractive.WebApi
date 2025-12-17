using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Dtos.Products;

namespace SiaInteractive.Application.Interfaces
{
    public interface IProductApplication
    {
        Task<Response<bool>> InsertAsync(CreateProductDto ProductDto);
        Task<Response<bool>> UpdateAsync(UpdateProductDto ProductDto);
        Task<Response<bool>> DeleteAsync(int productId);
        Task<Response<ProductDto>> GetAsync(int productId);
        Task<Response<IEnumerable<ProductDto>>> GetAllAsync();
        Task<ResponsePagination<IEnumerable<ProductDto>>> GetAllWithPaginationAsync(int pageNumber, int pageSize);
    }
}
