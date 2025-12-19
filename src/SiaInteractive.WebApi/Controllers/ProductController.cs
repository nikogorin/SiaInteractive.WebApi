using Microsoft.AspNetCore.Mvc;
using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SiaInteractive.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing products in the system.
    /// Provides endpoints for creating, updating, deleting, and retrieving products.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Product Management")]
    public class ProductController : ControllerBase
    {
        private readonly IProductApplication _productApplication;
        private readonly ILogger<ProductController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productApplication"></param>
        /// <param name="logger"></param>
        public ProductController(IProductApplication productApplication, ILogger<ProductController> logger)
        {
            _productApplication = productApplication;
            _logger = logger;
        }

        /// <summary>
        /// Inserts a new product asynchronously.
        /// </summary>
        /// <param name="productDto">The product data transfer object containing product details.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the insert operation.</returns>
        [HttpPost("InsertAsync")]
        [SwaggerOperation(Summary = "Insert a new product")]
        [SwaggerResponse(200, "Product inserted successfully", typeof(Response<bool>))]
        public async Task<IActionResult> InsertAsync([FromBody] CreateProductDto productDto, CancellationToken cancellationToken)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var response = await _productApplication.InsertAsync(productDto, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return Problem(title: "Request failed", detail: response.Message, statusCode: StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Updates an existing product asynchronously.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="ProductDto">The product data transfer object containing updated product details.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the update operation.</returns>
        [HttpPut("UpdateAsync/{productId}")]
        [SwaggerOperation(Summary = "Update an existing product")]
        [SwaggerResponse(200, "Product updated successfully", typeof(Response<bool>))]
        public async Task<IActionResult> UpdateAsync([FromRoute] int productId, [FromBody] UpdateProductDto ProductDto, CancellationToken cancellationToken)
        {
            if (ProductDto == null)
            {
                return BadRequest("Product data is null.");
            }

            if (!productId.Equals(ProductDto.Id))
            {
                return BadRequest("Product Id mismatch.");
            }

            var response = await _productApplication.UpdateAsync(ProductDto, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return Problem(title: "Request failed", detail: response.Message, statusCode: StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Deletes a product asynchronously by their ID.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the delete operation.</returns>
        [HttpDelete("DeleteAsync/{productId}")]
        [SwaggerOperation(Summary = "Delete a product")]
        [SwaggerResponse(200, "Product deleted successfully", typeof(Response<bool>))]
        public async Task<IActionResult> DeleteAsync([FromRoute] int productId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteAsync called for ProductId: {productId}", productId);
            if (productId <= 0)
            {
                return BadRequest("Product Id is invalid.");
            }

            var response = await _productApplication.DeleteAsync(productId, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return Problem(title: "Request failed", detail: response.Message, statusCode: StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Retrieves a product asynchronously by their ID.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get operation.</returns>
        [HttpGet("GetAsync/{productId}")]
        [SwaggerOperation(Summary = "Get a product by Id")]
        [SwaggerResponse(200, "Product retrieved successfully", typeof(Response<ProductDto>))]
        public async Task<IActionResult> GetAsync([FromRoute] int productId, CancellationToken cancellationToken)
        {
            if (productId <= 0)
            {
                return BadRequest("Product Id is null or empty.");
            }
            var response = await _productApplication.GetAsync(productId, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return Problem(title: "Request failed", detail: response.Message, statusCode: StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Retrieves all products asynchronously.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get all operation.</returns>
        [HttpGet("GetAllAsync")]
        [SwaggerOperation(Summary = "Get all product")]
        [SwaggerResponse(200, "Product retrieved successfully", typeof(Response<IEnumerable<ProductDto>>))]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _productApplication.GetAllAsync(cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return Problem(title: "Request failed", detail: response.Message, statusCode: StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Retrieves all products asynchronously with pagination. 
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get all operation.</returns>
        [HttpGet("GetAllWithPaginationAsync")]
        [SwaggerOperation(Summary = "Get all products with pagination")]
        [SwaggerResponse(200, "Categories retrieved successfully", typeof(Response<IEnumerable<ProductDto>>))]
        public async Task<IActionResult> GetAllWithPaginationAsync([FromQuery] int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            const int MaxPageSize = 1000;

            if (pageNumber < 1)
                return BadRequest(new { message = "pageNumber must be >= 1." });

            if (pageSize < 1 || pageSize > MaxPageSize)
                return BadRequest(new { message = $"pageSize must be between 1 and {MaxPageSize}." });

            var response = await _productApplication.GetAllWithPaginationAsync(pageNumber, pageSize, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return Problem(title: "Request failed", detail: response.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
