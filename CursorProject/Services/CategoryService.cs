using CursorProject.Data;  // Import database context for data access
using CursorProject.DTOs.Category;  // Import category data transfer objects
using CursorProject.DTOs;  // Import main DTOs for responses and requests
using CursorProject.Entities;  // Import domain entities
using Microsoft.EntityFrameworkCore;  // Import Entity Framework Core for database operations

namespace CursorProject.Services  // Define namespace for business logic services
{
    // Interface defining the contract for category service operations
    // This interface allows for dependency injection and unit testing
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();  // Get all available categories
        Task<CategoryDto?> GetCategoryByIdAsync(int id);  // Get single category by its unique identifier
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request);  // Create new category in the system
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequest request);  // Update existing category information
        Task<bool> DeleteCategoryAsync(int id);  // Delete category from the system
    }

    // Implementation of category service that handles category management
    // This class contains all business logic for category operations
    public class CategoryService : ICategoryService  // Implement the ICategoryService interface
    {
        // Dependency injection field for database context
        private readonly ApplicationDbContext _context;  // Database context for Entity Framework operations

        // Constructor for dependency injection of database context
        public CategoryService(ApplicationDbContext context)  // Inject database context
        {
            _context = context;  // Store database context reference
        }

        // Get all available categories in the system
        // This method provides category list for product organization and filtering
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            // Query all categories from database and transform to DTOs
            return await _context.Categories  // Get categories from database
                .Select(c => new CategoryDto  // Transform to DTOs
                {
                    Id = c.Id,  // Set category ID
                    Name = c.Name,  // Set category name
                    Description = c.Description  // Set category description
                })
                .ToListAsync();  // Execute query and return list
        }

        // Get a single category by its unique identifier
        // This method handles category detail viewing
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            // Query the database to find category by ID
            var category = await _context.Categories  // Get categories from database
                .FirstOrDefaultAsync(c => c.Id == id);  // Find category with matching ID

            // Return null if category is not found
            if (category == null)  // Check if category exists
                return null;  // Return null for non-existent categories

            // Transform database entity to DTO and return
            return new CategoryDto
            {
                Id = category.Id,  // Set category ID
                Name = category.Name,  // Set category name
                Description = category.Description  // Set category description
            };
        }

        // Create a new category in the system
        // This method handles category creation with validation
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request)
        {
            // Create new category entity from request data
            var category = new Category
            {
                Name = request.Name,  // Set category name from request
                Description = request.Description  // Set category description from request
            };

            // Add category to database context
            _context.Categories.Add(category);  // Mark category for insertion
            await _context.SaveChangesAsync();  // Save changes to database

            // Transform created entity to DTO and return
            return new CategoryDto
            {
                Id = category.Id,  // Set category ID (auto-generated)
                Name = category.Name,  // Set category name
                Description = category.Description  // Set category description
            };
        }

        // Update an existing category in the system
        // This method handles category information updates
        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            // Find existing category by ID
            var category = await _context.Categories  // Get categories from database
                .FirstOrDefaultAsync(c => c.Id == id);  // Find category with matching ID

            // Throw exception if category is not found
            if (category == null)  // Check if category exists
                throw new ArgumentException("Category not found");  // Throw exception for non-existent categories

            // Update category properties with new values from request
            category.Name = request.Name;  // Update category name
            category.Description = request.Description;  // Update category description

            // Save changes to database
            await _context.SaveChangesAsync();  // Persist changes to database

            // Transform updated entity to DTO and return
            return new CategoryDto
            {
                Id = category.Id,  // Set category ID
                Name = category.Name,  // Set updated category name
                Description = category.Description  // Set updated category description
            };
        }

        // Delete a category from the system
        // This method handles category removal with validation
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // Find category by ID
            var category = await _context.Categories  // Get categories from database
                .FirstOrDefaultAsync(c => c.Id == id);  // Find category with matching ID

            // Return false if category is not found
            if (category == null)  // Check if category exists
                return false;  // Return false for non-existent categories

            // Remove category from database context
            _context.Categories.Remove(category);  // Mark category for deletion
            await _context.SaveChangesAsync();  // Save changes to database

            return true;  // Return true for successful deletion
        }
    }
}
