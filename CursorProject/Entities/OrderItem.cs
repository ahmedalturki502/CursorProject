using System.ComponentModel.DataAnnotations;  // Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations.Schema;  // Import database schema attributes

namespace CursorProject.Entities  // Define namespace for all domain entities
{
    // OrderItem entity that represents a single item within a customer order
    // This class tracks individual products and their quantities in orders
    public class OrderItem
    {
        // Primary key for the order item entity
        // Auto-generated unique identifier for each order item in the database
        public int Id { get; set; }
        
        // Foreign key reference to the order this item belongs to
        // Links the order item to its parent order
        // This is the database foreign key column
        public int OrderId { get; set; }
        
        // Navigation property to the order this item belongs to
        // Virtual keyword enables Entity Framework lazy loading (order loaded only when accessed)
        // Many-to-one relationship: many order items can belong to one order
        // Null-forgiving operator (!) tells compiler this will not be null when accessed
        public virtual Order Order { get; set; } = null!;
        
        // Foreign key reference to the product in this order item
        // Links the order item to the specific product being ordered
        // This is the database foreign key column
        public int ProductId { get; set; }
        
        // Navigation property to the product in this order item
        // Virtual keyword enables Entity Framework lazy loading (product loaded only when accessed)
        // Many-to-one relationship: many order items can reference one product
        // Null-forgiving operator (!) tells compiler this will not be null when accessed
        public virtual Product Product { get; set; } = null!;
        
        // Quantity of the product ordered in this item
        // Required field that must be greater than 0 for valid order items
        // Used for inventory management and order calculations
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]  // Validation attribute: minimum value 1
        public int Quantity { get; set; }
        
        // Price of the product at the time of ordering
        // Required field that must be greater than 0 for valid order items
        // Uses decimal type for accurate financial calculations without rounding errors
        // This price is stored to maintain historical pricing even if product price changes later
        [Required]  // Validation attribute: field is mandatory and cannot be null
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]  // Validation attribute: minimum value 0.01
        [Column(TypeName = "decimal(18,2)")]  // Database column type specification for precise decimal storage
        public decimal Price { get; set; }
        
        // Subtotal for this order item (Quantity * Price)
        // Calculated property that provides the total cost for this specific item
        // Used for order total calculations and financial reporting
        [Column(TypeName = "decimal(18,2)")]  // Database column type specification for precise decimal storage
        public decimal Subtotal => Quantity * Price;  // Calculate subtotal as quantity times price
    }
}
