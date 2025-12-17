using Microsoft.AspNetCore.Mvc;
using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

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
        /// <returns>An <see cref="IActionResult"/> containing the result of the insert operation.</returns>
        [HttpPost("InsertAsync")]
        [SwaggerOperation(Summary = "Insert a new product")]
        [SwaggerResponse(200, "Product inserted successfully", typeof(Response<bool>))]
        public async Task<IActionResult> InsertAsync([FromBody] CreateProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var response = await _productApplication.InsertAsync(productDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }

        /// <summary>
        /// Updates an existing product asynchronously.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="ProductDto">The product data transfer object containing updated product details.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the update operation.</returns>
        [HttpPut("UpdateAsync/{productId}")]
        [SwaggerOperation(Summary = "Update an existing product")]
        [SwaggerResponse(200, "Product updated successfully", typeof(Response<bool>))]
        public async Task<IActionResult> UpdateAsync([FromRoute] int productId, [FromBody] UpdateProductDto ProductDto)
        {
            if (ProductDto == null)
            {
                return BadRequest("Product data is null.");
            }

            if (!productId.Equals(ProductDto.Id))
            {
                return BadRequest("Product Id mismatch.");
            }

            var response = await _productApplication.UpdateAsync(ProductDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }

        /// <summary>
        /// Deletes a product asynchronously by their ID.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the delete operation.</returns>
        [HttpDelete("DeleteAsync/{productId}")]
        [SwaggerOperation(Summary = "Delete a product")]
        [SwaggerResponse(200, "Product deleted successfully", typeof(Response<bool>))]
        public async Task<IActionResult> DeleteAsync([FromRoute] int productId)
        {
            _logger.LogInformation("DeleteAsync called for ProductId: {productId}", productId);
            if (productId <= 0)
            {
                return BadRequest("Product Id is invalid.");
            }

            var response = await _productApplication.DeleteAsync(productId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }

        /// <summary>
        /// Retrieves a product asynchronously by their ID.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get operation.</returns>
        [HttpGet("GetAsync/{productId}")]
        [SwaggerOperation(Summary = "Get a product by Id")]
        [SwaggerResponse(200, "Product retrieved successfully", typeof(Response<ProductDto>))]
        public async Task<IActionResult> GetAsync([FromRoute] int productId)
        {
            if (productId <= 0)
            {
                return BadRequest("Product Id is null or empty.");
            }
            var response = await _productApplication.GetAsync(productId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }

        /// <summary>
        /// Retrieves all products asynchronously.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get all operation.</returns>
        [HttpGet("GetAllAsync")]
        [SwaggerOperation(Summary = "Get all product")]
        [SwaggerResponse(200, "Product retrieved successfully", typeof(Response<IEnumerable<ProductDto>>))]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _productApplication.GetAllAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }

        /// <summary>
        /// Retrieves all products asynchronously with pagination. 
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get all operation.</returns>
        [HttpGet("GetAllWithPaginationAsync")]
        [SwaggerOperation(Summary = "Get all products with pagination")]
        [SwaggerResponse(200, "Categories retrieved successfully", typeof(Response<IEnumerable<ProductDto>>))]
        public async Task<IActionResult> GetAllWithPaginationAsync([FromQuery] int pageNumber, int pageSize)
        {
            var response = await _productApplication.GetAllWithPaginationAsync(pageNumber, pageSize);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }
    }
}
