using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Application.Dtos.Common;

namespace SiaInteractive.Application.Interfaces
{
    public interface ICategoryApplication
    {
        Task<Response<bool>> InsertAsync(CreateCategoryDto categoryDto);
        Task<Response<bool>> UpdateAsync(UpdateCategoryDto categoryDto);
        Task<Response<bool>> DeleteAsync(int categoryId);
        Task<Response<CategoryDto>> GetAsync(int categoryId);
        Task<Response<IEnumerable<CategoryDto>>> GetAllAsync();
        Task<ResponsePagination<IEnumerable<CategoryDto>>> GetAllWithPaginationAsync(int pageNumber, int pageSize);
    }
}
