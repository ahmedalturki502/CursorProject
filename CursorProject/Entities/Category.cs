using System.ComponentModel.DataAnnotations;  // Import data validation attributes for property validation

namespace CursorProject.Entities  // Define namespace for all domain entities
{
    // Category entity that represents a product category in the e-commerce system
    // This class organizes products into logical groups for better navigation and filtering
    public class Category
    {
        // Primary key for the category entity
        // Auto-generated unique identifier for each category in the database
        public int Id { get; set; }
        
        // Name of the category (e.g., "Electronics", "Clothing", "Books")
        // Required field that must not be null or empty for category identification
        // Maximum length of 100 characters to prevent overly long category names
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [StringLength(100)]  // Validation attribute: maximum 100 characters allowed
        public string Name { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Detailed description of the category and what products it contains
        // Optional field that can be empty if no description is provided
        // Maximum length of 500 characters for comprehensive category descriptions
        [StringLength(500)]  // Validation attribute: maximum 500 characters allowed
        public string Description { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Collection of products that belong to this category
        // Virtual keyword enables Entity Framework lazy loading (products loaded only when accessed)
        // One-to-many relationship: one category can contain many products
        // Initialized as empty list to avoid null reference exceptions
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
