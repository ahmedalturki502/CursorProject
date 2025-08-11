// Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations;

// Namespace for all application models
namespace CursorProject.Models
{
    /// <summary>
    /// Represents a customer order in the e-commerce system
    /// Contains order details, customer information, and shipping details
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Primary key for the order
        /// Auto-generated unique identifier for each order
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key reference to the user who placed this order
        /// Links the order to the customer who made the purchase
        /// </summary>
        [Required]  // Validation: field is mandatory
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Navigation property to the user who placed this order
        /// Virtual property for Entity Framework lazy loading
        /// Many-to-one relationship: many orders can belong to one user
        /// </summary>
        public virtual ApplicationUser User { get; set; } = null!;
        
        /// <summary>
        /// Date and time when the order was placed
        /// Required field that defaults to current UTC time
        /// Uses UTC to avoid timezone issues
        /// </summary>
        [Required]  // Validation: field is mandatory
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Total amount of the order in decimal format for precise currency handling
        /// Required field that must be greater than 0
        /// Calculated from the sum of all order items
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]  // Validation: minimum 0.01
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// Shipping address where the order will be delivered
        /// Required field with minimum length of 10 characters for meaningful addresses
        /// Maximum length of 500 characters for complete address information
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Shipping address must be at least 10 characters")]  // Validation: 10-500 characters
        public string ShippingAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Collection of items in this order
        /// Virtual property for Entity Framework lazy loading
        /// One-to-many relationship: one order can have many order items
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
