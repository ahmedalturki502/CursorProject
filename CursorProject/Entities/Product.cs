using System.ComponentModel.DataAnnotations;  // Import data validation attributes for property validation

namespace CursorProject.Entities  // Define namespace for all domain entities
{
    // Product entity that represents a product in the e-commerce system
    // This class contains all essential information about products that customers can purchase
    public class Product
    {
        // Primary key for the product entity
        // Auto-generated unique identifier for each product in the database
        public int Id { get; set; }
        
        // Name of the product (e.g., "iPhone 15", "Nike Running Shoes")
        // Required field that must not be null or empty for product identification
        // Maximum length of 200 characters to prevent overly long product names
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [StringLength(200)]  // Validation attribute: maximum 200 characters allowed
        public string Name { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Detailed description of the product features, specifications, and benefits
        // Optional field that can be empty if no description is provided
        // Maximum length of 1000 characters for comprehensive product descriptions
        [StringLength(1000)]  // Validation attribute: maximum 1000 characters allowed
        public string Description { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Price of the product in decimal format for precise currency handling
        // Required field that must be greater than 0 (no free products allowed)
        // Uses decimal type for accurate financial calculations without rounding errors
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]  // Validation attribute: minimum value 0.01
        public decimal Price { get; set; }
        
        // URL to the product image for display purposes in the user interface
        // Optional field that can be empty if no image is available
        // Maximum length of 500 characters for image URLs
        [StringLength(500)]  // Validation attribute: maximum 500 characters allowed
        public string ImageUrl { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Current stock quantity available for purchase (inventory management)
        // Required field that must be 0 or greater (can be out of stock)
        // Used for inventory management and order validation to prevent overselling
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]  // Validation attribute: minimum value 0
        public int StockQuantity { get; set; }
        
        // Foreign key reference to the category this product belongs to
        // Links the product to its category for organization and filtering
        // This is the database foreign key column
        public int CategoryId { get; set; }
        
        // Navigation property to the category this product belongs to
        // Virtual keyword enables Entity Framework lazy loading (category loaded only when accessed)
        // Many-to-one relationship: many products can belong to one category
        // Null-forgiving operator (!) tells compiler this will not be null when accessed
        public virtual Category Category { get; set; } = null!;
        
        // Collection of order items that contain this product
        // Virtual keyword enables Entity Framework lazy loading (order items loaded only when accessed)
        // One-to-many relationship: one product can be in many order items
        // Initialized as empty list to avoid null reference exceptions
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Collection of cart items that contain this product
        // Virtual keyword enables Entity Framework lazy loading (cart items loaded only when accessed)
        // One-to-many relationship: one product can be in many cart items
        // Initialized as empty list to avoid null reference exceptions
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
