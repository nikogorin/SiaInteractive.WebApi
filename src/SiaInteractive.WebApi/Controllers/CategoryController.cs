using Microsoft.AspNetCore.Mvc;
using SiaInteractive.Application.Dtos.Categories;
using SiaInteractive.Application.Dtos.Common;
using SiaInteractive.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SiaInteractive.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing categories in the system.
    /// Provides endpoints for creating, updating, deleting, and retrieving categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Category Management")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryApplication _categoryApplication;
        private readonly ILogger<CategoryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryApplication"></param>
        /// <param name="logger"></param>
        public CategoryController(ICategoryApplication categoryApplication, ILogger<CategoryController> logger)
        {
            _categoryApplication = categoryApplication;
            _logger = logger;
        }

        /// <summary>
        /// Inserts a new category asynchronously.
        /// </summary>
        /// <param name="categoryDto">The category data transfer object containing category details.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the insert operation.</returns>
        [HttpPost("InsertAsync")]
        [SwaggerOperation(Summary = "Insert a new category")]
        [SwaggerResponse(200, "Category inserted successfully", typeof(Response<bool>))]
        public async Task<IActionResult> InsertAsync([FromBody] CreateCategoryDto categoryDto, CancellationToken cancellationToken)
        {
            if (categoryDto == null)
            {
                return BadRequest("Category data is null.");
            }

            var response = await _categoryApplication.InsertAsync(categoryDto, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        /// <summary>
        /// Updates an existing category asynchronously.
        /// </summary>
        /// <param name="categoryId">The ID of the category to update.</param>
        /// <param name="categoryDto">The category data transfer object containing updated category details.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the update operation.</returns>
        [HttpPut("UpdateAsync/{categoryId}")]
        [SwaggerOperation(Summary = "Update an existing category")]
        [SwaggerResponse(200, "Category updated successfully", typeof(Response<bool>))]
        public async Task<IActionResult> UpdateAsync([FromRoute] int categoryId, [FromBody] UpdateCategoryDto categoryDto, CancellationToken cancellationToken)
        {
            if (categoryDto == null)
            {
                return BadRequest("Category data is null.");
            }

            if (!categoryId.Equals(categoryDto.Id))
            {
                return BadRequest("Category Id mismatch.");
            }

            var response = await _categoryApplication.UpdateAsync(categoryDto, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        /// <summary>
        /// Deletes a category asynchronously by their ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the delete operation.</returns>
        [HttpDelete("DeleteAsync/{categoryId}")]
        [SwaggerOperation(Summary = "Delete a category")]
        [SwaggerResponse(200, "Category deleted successfully", typeof(Response<bool>))]
        public async Task<IActionResult> DeleteAsync([FromRoute] int categoryId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteAsync called for CategoryId: {CategoryId}", categoryId);
            if (categoryId <= 0)
            {
                return BadRequest("Category Id is invalid.");
            }

            var response = await _categoryApplication.DeleteAsync(categoryId, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return NotFound(response.Message);
        }

        /// <summary>
        /// Retrieves a category asynchronously by their ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get operation.</returns>
        [HttpGet("GetAsync/{categoryId}")]
        [SwaggerOperation(Summary = "Get a category by Id")]
        [SwaggerResponse(200, "Category retrieved successfully", typeof(Response<CategoryDto>))]
        public async Task<IActionResult> GetAsync([FromRoute] int categoryId, CancellationToken cancellationToken)
        {
            if (categoryId <= 0)
            {
                return BadRequest("Category Id is null or empty.");
            }
            var response = await _categoryApplication.GetAsync(categoryId, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response.Message);
        }

        /// <summary>
        /// Retrieves all categories asynchronously.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get all operation.</returns>
        [HttpGet("GetAllAsync")]
        [SwaggerOperation(Summary = "Get all category")]
        [SwaggerResponse(200, "Category retrieved successfully", typeof(Response<IEnumerable<CategoryDto>>))]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _categoryApplication.GetAllAsync(cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        /// <summary>
        /// Retrieves all categories asynchronously with pagination. 
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the result of the get all operation.</returns>
        [HttpGet("GetAllWithPaginationAsync")]
        [SwaggerOperation(Summary = "Get all categories with pagination")]
        [SwaggerResponse(200, "Categories retrieved successfully", typeof(Response<IEnumerable<CategoryDto>>))]
        public async Task<IActionResult> GetAllWithPaginationAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            const int MaxPageSize = 1000;

            if (pageNumber < 1)
                return BadRequest(new { message = "pageNumber must be >= 1." });

            if (pageSize < 1 || pageSize > MaxPageSize)
                return BadRequest(new { message = $"pageSize must be between 1 and {MaxPageSize}." });

            var response = await _categoryApplication.GetAllWithPaginationAsync(pageNumber, pageSize, cancellationToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }
    }
}
