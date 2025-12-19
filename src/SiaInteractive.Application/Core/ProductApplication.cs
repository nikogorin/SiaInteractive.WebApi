using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SiaInteractive.Abstractions.Interfaces;
using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Application.Interfaces;
using SiaInteractive.Domain.Entities;

namespace SiaInteractive.Application.Core
{
    public class ProductApplication : IProductApplication
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CreateProductDto> _createProductValidator;
        private readonly IValidator<UpdateProductDto> _updateProductValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductApplication> _logger;

        public ProductApplication(IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IValidator<CreateProductDto> createProductValidator,
            IValidator<UpdateProductDto> updateProductValidator,
            IMapper mapper, 
            ILogger<ProductApplication> logger) 
        { 
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ProductDto>> GetAsync(int productId, CancellationToken cancellationToken)
        {
            var response = new Response<ProductDto>();

            var product = await _productRepository.GetAsync(productId, cancellationToken);
            response.Data = _mapper.Map<ProductDto>(product);
            if (response.Data != null)
            {
                response.IsSuccess = true;
                response.Message = "Product retrieved successfully.";
            }
            else
            {
                response.Message = "Product not found.";
            }

            return response;
        }

        public async Task<Response<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = new Response<IEnumerable<ProductDto>>();

            var products = await _productRepository.GetAllAsync(cancellationToken);
            response.Data = _mapper.Map<IEnumerable<ProductDto>>(products);
            if (response.Data != null)
            {
                response.IsSuccess = true;
                response.Message = "Products retrieved successfully.";
            }
            else
            {
                response.Message = "No products found.";
            }

            return response;
        }

        public async Task<ResponsePagination<IEnumerable<ProductDto>>> GetAllWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var response = new ResponsePagination<IEnumerable<ProductDto>>();

            var count = await _productRepository.Count(cancellationToken);
            var products = await _productRepository.GetAllWithPaginationAsync(pageNumber, pageSize, cancellationToken);
            response.Data = _mapper.Map<IEnumerable<ProductDto>>(products);
            if (response.Data != null)
            {
                response.PageNumber = pageNumber;
                response.TotalRecords = count;
                response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                response.IsSuccess = true;
                response.Message = "Products retrieved successfully.";
            }
            else
            {
                response.Message = "No products found.";
            }

            return response;
        }

        public async Task<Response<bool>> InsertAsync(CreateProductDto productDto, CancellationToken cancellationToken)
        {
            var response = new Response<bool>();

            var validation = await _createProductValidator.ValidateAsync(productDto, cancellationToken);
            if (!validation.IsValid)
            {
                response.IsSuccess = false;
                response.Message = "Validation errors occurred.";
                response.ValidationErrors = validation.Errors;
                return response;
            }

            var product = _mapper.Map<Product>(productDto);
            var categories = await _categoryRepository.GetTrackingAsync(productDto.CategoryIds!, cancellationToken);
            product.Categories = categories.ToList()!;

            response.Data = await _productRepository.InsertAsync(product, cancellationToken);
            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = "Product inserted successfully.";
            }
            else
            {
                response.Message = "Failed to insert product.";
            }

            return response;
        }

        public async Task<Response<bool>> UpdateAsync(UpdateProductDto productDto, CancellationToken cancellationToken)
        {
            var response = new Response<bool>();

            var validation = await _updateProductValidator.ValidateAsync(productDto, cancellationToken);
            if (!validation.IsValid)
            {
                response.IsSuccess = false;
                response.Message = "Validation errors occurred.";
                response.ValidationErrors = validation.Errors;
                return response;
            }

            var savedProduct = await _productRepository.GetTrackingAsync(productDto.Id, cancellationToken);
            savedProduct = _mapper.Map(productDto, savedProduct);
            var requestedCategories = await _categoryRepository.GetTrackingAsync(productDto.CategoryIds!, cancellationToken);

            CategoriesToRemove(productDto, savedProduct);
            CategoriesToAdd(savedProduct, requestedCategories);

            response.Data = await _productRepository.UpdateAsync(savedProduct, cancellationToken);
            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = "Product updated successfully.";
            }
            else
            {
                response.Message = "Failed to uptate product.";
            }

            return response;
        }

        public async Task<Response<bool>> DeleteAsync(int productId, CancellationToken cancellationToken)
        {
            var response = new Response<bool>
            {
                Data = await _productRepository.DeleteAsync(productId, cancellationToken)
            };
            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = "Product deleted successfully.";

                _logger.LogInformation("Product deleted successfully");
            }
            else
            {
                response.Message = "Failed to delete product.";
            }

            return response;
        }

        private static void CategoriesToRemove(UpdateProductDto productDto, Product savedProduct)
        {
            var categoriesToRemove = savedProduct!.Categories.Where(c => !productDto.CategoryIds!.Contains(c.CategoryID)).ToList();
            foreach (var category in categoriesToRemove)
                savedProduct.Categories.Remove(category);
        }

        private static void CategoriesToAdd(Product savedProduct, IEnumerable<Category?> requestedCategories)
        {
            var currentCategoryIds = savedProduct.Categories.Select(c => c.CategoryID).ToHashSet();
            var categoriesToAdd = requestedCategories.Where(c => !currentCategoryIds.Contains(c.CategoryID));
            foreach (var category in categoriesToAdd)
                savedProduct.Categories.Add(category!);
        }
    }
}
