// Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations;

// Namespace for all application models
namespace CursorProject.Models
{
    /// <summary>
    /// Represents an individual item within a customer order
    /// Links a specific product with its quantity and price at the time of purchase
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Primary key for the order item
        /// Auto-generated unique identifier for each order item
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key reference to the order this item belongs to
        /// Links the order item to its parent order
        /// </summary>
        [Required]  // Validation: field is mandatory
        public int OrderId { get; set; }
        
        /// <summary>
        /// Navigation property to the order this item belongs to
        /// Virtual property for Entity Framework lazy loading
        /// Many-to-one relationship: many order items can belong to one order
        /// </summary>
        public virtual Order Order { get; set; } = null!;
        
        /// <summary>
        /// Foreign key reference to the product in this order item
        /// Links the order item to the specific product that was purchased
        /// </summary>
        [Required]  // Validation: field is mandatory
        public int ProductId { get; set; }
        
        /// <summary>
        /// Navigation property to the product in this order item
        /// Virtual property for Entity Framework lazy loading
        /// Many-to-one relationship: many order items can reference one product
        /// </summary>
        public virtual Product Product { get; set; } = null!;
        
        /// <summary>
        /// Quantity of the product ordered
        /// Required field that must be at least 1 (cannot order 0 or negative items)
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]  // Validation: minimum 1
        public int Quantity { get; set; }
        
        /// <summary>
        /// Price of the product at the time of purchase
        /// Required field that must be greater than 0
        /// Stored separately from product price to handle price changes over time
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]  // Validation: minimum 0.01
        public decimal Price { get; set; }
    }
}
