// Import necessary namespaces for the categories controller
using CursorProject.Data;                      // Database context
using CursorProject.DTOs;                      // Data transfer objects
using CursorProject.Models;                    // Application models
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes
using Microsoft.EntityFrameworkCore;         // Entity Framework Core

// Namespace for API controllers
namespace CursorProject.Controllers
{
    /// <summary>
    /// Controller for handling category-related operations
    /// Provides endpoints for listing, creating, updating, and deleting product categories
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/categories
    public class CategoriesController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Database context for accessing category data
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor that accepts database context via dependency injection
        /// </summary>
        /// <param name="context">Database context for data access</param>
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;  // Store database context reference
        }

        /// <summary>
        /// Gets all categories with their product counts
        /// GET: api/categories
        /// </summary>
        /// <returns>List of all categories</returns>
        [HttpGet]  // HTTP GET endpoint
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            // Get all categories with product count information
            var categories = await _context.Categories
                .Select(c => new CategoryDto  // Project to DTO for response
                {
                    Id = c.Id,                // Category ID
                    Name = c.Name,            // Category name
                    ProductCount = c.Products.Count  // Count of products in this category
                })
                .ToListAsync();               // Execute query and get results

            return Ok(categories);  // Return list of categories
        }

        /// <summary>
        /// Gets a specific category by ID with product count
        /// GET: api/categories/{id}
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id}")]  // HTTP GET endpoint with ID parameter
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            // Find category by ID including products for counting
            var category = await _context.Categories
                .Include(c => c.Products)     // Include products for counting
                .FirstOrDefaultAsync(c => c.Id == id);  // Find category by ID

            // Return 404 if category not found
            if (category == null)
            {
                return NotFound();
            }

            // Return category details as DTO
            return Ok(new CategoryDto
            {
                Id = category.Id,             // Category ID
                Name = category.Name,         // Category name
                ProductCount = category.Products.Count  // Count of products in this category
            });
        }

        /// <summary>
        /// Creates a new category (Admin only)
        /// POST: api/categories
        /// </summary>
        /// <param name="request">Category creation request</param>
        /// <returns>Created category details</returns>
        [HttpPost]  // HTTP POST endpoint
        [Authorize(Roles = "Admin")]  // Requires Admin role
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryRequest request)
        {
            // Check if category name already exists (names must be unique)
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == request.Name);
            if (existingCategory != null)
            {
                return BadRequest(new { Message = "Category name already exists" });  // Return 400 if name taken
            }

            // Create new category entity
            var category = new Category
            {
                Name = request.Name  // Category name
            };

            // Add category to database
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();  // Save changes to database

            // Return 201 Created with category details and location header
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new CategoryDto
            {
                Id = category.Id,             // Category ID
                Name = category.Name,         // Category name
                ProductCount = 0              // New category has no products yet
            });
        }

        /// <summary>
        /// Updates an existing category (Admin only)
        /// PUT: api/categories/{id}
        /// </summary>
        /// <param name="id">Category ID to update</param>
        /// <param name="request">Category update request</param>
        /// <returns>Updated category details</returns>
        [HttpPut("{id}")]  // HTTP PUT endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Requires Admin role
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            // Find category by ID
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();  // Return 404 if category not found
            }

            // Check if category name already exists (excluding current category)
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id != id);
            if (existingCategory != null)
            {
                return BadRequest(new { Message = "Category name already exists" });  // Return 400 if name taken
            }

            // Update category name
            category.Name = request.Name;
            await _context.SaveChangesAsync();  // Save changes to database

            // Return updated category details
            return Ok(new CategoryDto
            {
                Id = category.Id,             // Category ID
                Name = category.Name,         // Updated name
                ProductCount = category.Products.Count  // Count of products in this category
            });
        }

        /// <summary>
        /// Deletes a category (Admin only)
        /// DELETE: api/categories/{id}
        /// </summary>
        /// <param name="id">Category ID to delete</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]  // HTTP DELETE endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Requires Admin role
        public async Task<ActionResult> DeleteCategory(int id)
        {
            // Find category by ID including products for validation
            var category = await _context.Categories
                .Include(c => c.Products)     // Include products to check if category has any
                .FirstOrDefaultAsync(c => c.Id == id);

            // Return 404 if category not found
            if (category == null)
            {
                return NotFound();
            }

            // Check if category has products (prevent deletion of categories with products)
            if (category.Products.Any())
            {
                return BadRequest(new { Message = "Cannot delete category that has products" });  // Return 400 if category has products
            }

            // Remove category from database
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();  // Save changes to database

            return NoContent();  // Return 204 No Content on successful deletion
        }
    }
}
