using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SiaInteractive.Abstractions.Interfaces;
using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Interfaces;
using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Application.Core
{
    public class CategoryApplication : ICategoryApplication
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CreateCategoryDto> _createCategoryValidator;
        private readonly IValidator<UpdateCategoryDto> _updateCategoryValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryApplication> _logger;

        public CategoryApplication(ICategoryRepository categoryRepository, 
            IValidator<CreateCategoryDto> createCategoryValidator, 
            IValidator<UpdateCategoryDto> updateCategoryValidator, 
            IMapper mapper, 
            ILogger<CategoryApplication> logger) 
        { 
            _categoryRepository = categoryRepository;
            _createCategoryValidator = createCategoryValidator;
            _updateCategoryValidator = updateCategoryValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<CategoryDto>> GetAsync(int categoryId, CancellationToken cancellationToken)
        {
            var response = new Response<CategoryDto>();

            var category = await _categoryRepository.GetAsync(categoryId, cancellationToken);
            response.Data = _mapper.Map<CategoryDto>(category);
            if (response.Data != null)
            {
                response.IsSuccess = true;
                response.Message = "Category retrieved successfully.";
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "Category not found.";
            }

            return response;
        }

        public async Task<Response<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = new Response<IEnumerable<CategoryDto>>();

            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            response.Data = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            if (response.Data != null && response.Data.Any())
            {
                response.IsSuccess = true;
                response.Message = "Categories retrieved successfully.";
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "No categories found.";
            }
            return response;
        }

        public async Task<ResponsePagination<IEnumerable<CategoryDto>>> GetAllWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var response = new ResponsePagination<IEnumerable<CategoryDto>>();

            var count = await _categoryRepository.Count(cancellationToken);
            var categories = await _categoryRepository.GetAllWithPaginationAsync(pageNumber, pageSize, cancellationToken);
            response.Data = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            if (response.Data != null)
            {
                response.PageNumber = pageNumber;
                response.TotalRecords = count;
                response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                response.IsSuccess = true;
                response.Message = "Categories retrieved successfully.";
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "No categories found.";
            }

            return response;
        }

        public async Task<Response<bool>> InsertAsync(CreateCategoryDto categoryDto, CancellationToken cancellationToken)
        {
            var response = new Response<bool>();

            var validation = await _createCategoryValidator.ValidateAsync(categoryDto, cancellationToken);
            if (!validation.IsValid)
            {
                response.IsSuccess = false;
                response.Message = "Validation errors occurred.";
                response.ValidationErrors = validation.Errors;
                return response;
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.CategoryID = 0;
            response.Data = await _categoryRepository.InsertAsync(category, cancellationToken);
            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = "Category inserted successfully.";
            }
            else
            {
                response.Message = "Failed to insert category.";
            }

            return response;
        }

        public async Task<Response<bool>> UpdateAsync(UpdateCategoryDto categoryDto, CancellationToken cancellationToken)
        {
            var response = new Response<bool>();

            var validation = await _updateCategoryValidator.ValidateAsync(categoryDto, cancellationToken);
            if (!validation.IsValid)
            {
                response.IsSuccess = false;
                response.Message = "Validation errors occurred.";
                response.ValidationErrors = validation.Errors;
                return response;
            }

            var category = _mapper.Map<Category>(categoryDto);
            response.Data = await _categoryRepository.UpdateAsync(category, cancellationToken);
            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = "Category updated successfully.";
            }
            else
            {
                response.Message = "Failed to uptate category.";
            }

            return response;
        }

        public async Task<Response<bool>> DeleteAsync(int categoryId, CancellationToken cancellationToken)
        {
            var response = new Response<bool>();

            var category = await _categoryRepository.GetAsync(categoryId, cancellationToken);
            if (category == null)
            {
                response.Message = "Category not found.";
                return response;
            }

            response.Data = await _categoryRepository.DeleteAsync(categoryId, cancellationToken);
            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = "Category deleted successfully.";

                _logger.LogInformation("Category deleted successfully");
            }
            else
            {
                response.Message = "Failed to delete category.";
            }

            return response;
        }
    }
}
