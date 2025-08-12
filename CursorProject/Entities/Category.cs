// Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations;

// Namespace for all application models
namespace CursorProject.Entities
{
    /// <summary>
    /// Represents a product category in the e-commerce system
    /// Categories help organize products for better browsing and management
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Primary key for the category
        /// Auto-generated unique identifier for each category
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the category (e.g., "Electronics", "Clothing", "Books")
        /// Required field that must not be null or empty
        /// Maximum length of 100 characters to prevent overly long names
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
        
        /// <summary>
        /// Collection of products that belong to this category
        /// Virtual property for Entity Framework lazy loading
        /// One-to-many relationship: one category can have many products
        /// </summary>
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
