using System.ComponentModel.DataAnnotations;  // Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations.Schema;  // Import database schema attributes

namespace CursorProject.Entities  // Define namespace for all domain entities
{
    // Order entity that represents a customer order in the e-commerce system
    // This class tracks the complete order lifecycle from creation to delivery
    public class Order
    {
        // Primary key for the order entity
        // Auto-generated unique identifier for each order in the database
        public int Id { get; set; }
        
        // Foreign key reference to the user who placed this order
        // Links the order to the customer who made the purchase
        // This is the database foreign key column
        public string UserId { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Navigation property to the user who placed this order
        // Virtual keyword enables Entity Framework lazy loading (user loaded only when accessed)
        // Many-to-one relationship: many orders can belong to one user
        // Null-forgiving operator (!) tells compiler this will not be null when accessed
        public virtual ApplicationUser User { get; set; } = null!;
        
        // Current status of the order (e.g., Pending, Processing, Shipped, Delivered)
        // Required field that tracks the order through its lifecycle
        // Uses enum for type safety and consistent status values
        [Required]  // Validation attribute: field is mandatory and cannot be null
        public OrderStatus Status { get; set; }  // Default value is first enum value (Pending)
        
        // Total amount of the order including all items and any applicable discounts
        // Required field that must be greater than 0 for valid orders
        // Uses decimal type for accurate financial calculations without rounding errors
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [Range(0.01, double.MaxValue, ErrorMessage = "Order total must be greater than 0")]  // Validation attribute: minimum value 0.01
        [Column(TypeName = "decimal(18,2)")]  // Database column type specification for precise decimal storage
        public decimal TotalAmount { get; set; }
        
        // Date and time when the order was created
        // Auto-set to current UTC time when order is created
        // Used for order tracking and reporting purposes
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;  // Initialize to current UTC time
        
        // Date and time when the order was last updated
        // Auto-updated whenever order properties are modified
        // Used for tracking order changes and audit purposes
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;  // Initialize to current UTC time
        
        // Shipping address for order delivery
        // Required field that must not be null or empty for order fulfillment
        // Maximum length of 500 characters for complete address information
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [StringLength(500)]  // Validation attribute: maximum 500 characters allowed
        public string ShippingAddress { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Customer's phone number for delivery contact
        // Required field for delivery coordination and customer service
        // Maximum length of 20 characters for international phone numbers
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [StringLength(20)]  // Validation attribute: maximum 20 characters allowed
        public string PhoneNumber { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Additional notes or special instructions for the order
        // Optional field that can be empty if no special instructions
        // Maximum length of 1000 characters for detailed instructions
        [StringLength(1000)]  // Validation attribute: maximum 1000 characters allowed
        public string? Notes { get; set; }  // Nullable string for optional notes
        
        // Collection of order items that make up this order
        // Virtual keyword enables Entity Framework lazy loading (items loaded only when accessed)
        // One-to-many relationship: one order can contain many order items
        // Initialized as empty list to avoid null reference exceptions
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
