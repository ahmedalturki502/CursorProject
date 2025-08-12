// Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations;

// Namespace for all application models
namespace CursorProject.Entities
{
    /// <summary>
    /// Represents a product in the e-commerce system
    /// Contains all the essential information about a product that customers can purchase
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Primary key for the product
        /// Auto-generated unique identifier for each product
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the product (e.g., "iPhone 15", "Nike Running Shoes")
        /// Required field that must not be null or empty
        /// Maximum length of 200 characters for product names
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(200)]  // Validation: maximum 200 characters
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Detailed description of the product
        /// Optional field that can be empty
        /// Maximum length of 1000 characters for detailed descriptions
        /// </summary>
        [StringLength(1000)]  // Validation: maximum 1000 characters
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Price of the product in decimal format for precise currency handling
        /// Required field that must be greater than 0 (no free products)
        /// Uses decimal for accurate financial calculations
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]  // Validation: minimum 0.01
        public decimal Price { get; set; }
        
        /// <summary>
        /// URL to the product image for display purposes
        /// Optional field that can be empty
        /// Maximum length of 500 characters for image URLs
        /// </summary>
        [StringLength(500)]  // Validation: maximum 500 characters
        public string ImageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Current stock quantity available for purchase
        /// Required field that must be 0 or greater (can be out of stock)
        /// Used for inventory management and order validation
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]  // Validation: minimum 0
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// Foreign key reference to the category this product belongs to
        /// Links the product to its category for organization
        /// </summary>
        public int CategoryId { get; set; }
        
        /// <summary>
        /// Navigation property to the category this product belongs to
        /// Virtual property for Entity Framework lazy loading
        /// Many-to-one relationship: many products can belong to one category
        /// </summary>
        public virtual Category Category { get; set; } = null!;
        
        /// <summary>
        /// Collection of order items that contain this product
        /// Virtual property for Entity Framework lazy loading
        /// One-to-many relationship: one product can be in many order items
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        /// <summary>
        /// Collection of cart items that contain this product
        /// Virtual property for Entity Framework lazy loading
        /// One-to-many relationship: one product can be in many cart items
        /// </summary>
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
