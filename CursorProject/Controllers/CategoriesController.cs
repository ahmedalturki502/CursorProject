// Import necessary namespaces for the categories controller
using CursorProject.DTOs.Category;             // Data transfer objects
using CursorProject.DTOs;  // Import main DTOs for responses and requests
using CursorProject.Services;                  // Custom services (CategoryService)
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes

// Namespace for API controllers
namespace CursorProject.Controllers
{
    /// <summary>
    /// Controller for handling category-related operations
    /// Provides endpoints for managing product categories
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/categories
    public class CategoriesController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Category service for handling category operations
        /// </summary>
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Constructor that accepts category service via dependency injection
        /// </summary>
        /// <param name="categoryService">Category service for category operations</param>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;  // Store category service reference
        }

        /// <summary>
        /// Gets all available categories
        /// GET: api/categories
        /// </summary>
        /// <returns>List of all categories</returns>
        [HttpGet]  // HTTP GET endpoint
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Gets a specific category by ID
        /// GET: api/categories/{id}
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id}")]  // HTTP GET endpoint with ID parameter
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();  // Return 404 if category not found
            }

            return Ok(category);
        }

        /// <summary>
        /// Creates a new category
        /// POST: api/categories
        /// Requires admin authorization
        /// </summary>
        /// <param name="request">Category creation request</param>
        /// <returns>Created category details</returns>
        [HttpPost]  // HTTP POST endpoint
        [Authorize(Roles = "Admin")]  // Require admin role
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto request)
        {
            var category = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        /// <summary>
        /// Updates an existing category
        /// PUT: api/categories/{id}
        /// Requires admin authorization
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="request">Category update request</param>
        /// <returns>Updated category details</returns>
        [HttpPut("{id}")]  // HTTP PUT endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Require admin role
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, request);
                return Ok(category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a category
        /// DELETE: api/categories/{id}
        /// Requires admin authorization
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]  // HTTP DELETE endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Require admin role
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _categoryService.DeleteCategoryAsync(id);
                if (!success)
                {
                    return NotFound();  // Return 404 if category not found
                }

                return NoContent();  // Return 204 for successful deletion
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
