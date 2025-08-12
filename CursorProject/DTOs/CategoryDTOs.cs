// Import data validation attributes for input validation
using System.ComponentModel.DataAnnotations;

// Namespace for Data Transfer Objects (DTOs)
namespace CursorProject.DTOs
{
    /// <summary>
    /// Data transfer object for creating new categories
    /// Contains the required information to create a product category
    /// </summary>
    public class CreateCategoryRequest
    {
        /// <summary>
        /// Name of the category (e.g., "Electronics", "Clothing", "Books")
        /// Required field with maximum length of 100 characters
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(100)]  // Validation: maximum 100 characters
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the category
        /// Optional field with maximum length of 500 characters
        /// </summary>
        [StringLength(500)]  // Validation: maximum 500 characters
        public string? Description { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating existing categories
    /// Contains the field that can be modified for a category
    /// </summary>
    public class UpdateCategoryRequest
    {
        /// <summary>
        /// Updated name of the category
        /// Required field with maximum length of 100 characters
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(100)]  // Validation: maximum 100 characters
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Updated description of the category
        /// Optional field with maximum length of 500 characters
        /// </summary>
        [StringLength(500)]  // Validation: maximum 500 characters
        public string? Description { get; set; }
    }

    /// <summary>
    /// Data transfer object for category information in responses
    /// Contains complete category details including product count
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// Unique identifier for the category
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the category
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Number of products currently in this category
        /// Used for display purposes and category management
        /// </summary>
        public int ProductCount { get; set; }
    }
}
