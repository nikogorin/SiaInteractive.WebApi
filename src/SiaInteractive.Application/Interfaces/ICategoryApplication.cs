using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Application.Dtos.Common;

namespace SiaInteractive.Application.Interfaces
{
    public interface ICategoryApplication
    {
        Task<Response<bool>> InsertAsync(CreateCategoryDto categoryDto, CancellationToken cancellationToken);
        Task<Response<bool>> UpdateAsync(UpdateCategoryDto categoryDto, CancellationToken cancellationToken);
        Task<Response<bool>> DeleteAsync(int categoryId, CancellationToken cancellationToken);
        Task<Response<CategoryDto>> GetAsync(int categoryId, CancellationToken cancellationToken);
        Task<Response<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<ResponsePagination<IEnumerable<CategoryDto>>> GetAllWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
