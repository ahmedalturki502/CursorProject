// Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations;

// Namespace for all application models
namespace CursorProject.Models
{
    /// <summary>
    /// Represents an individual item within a shopping cart
    /// Links a specific product with its quantity in the user's cart
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Primary key for the cart item
        /// Auto-generated unique identifier for each cart item
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key reference to the cart this item belongs to
        /// Links the cart item to its parent cart
        /// </summary>
        [Required]  // Validation: field is mandatory
        public int CartId { get; set; }
        
        /// <summary>
        /// Navigation property to the cart this item belongs to
        /// Virtual property for Entity Framework lazy loading
        /// Many-to-one relationship: many cart items can belong to one cart
        /// </summary>
        public virtual Cart Cart { get; set; } = null!;
        
        /// <summary>
        /// Foreign key reference to the product in this cart item
        /// Links the cart item to the specific product that was added to cart
        /// </summary>
        [Required]  // Validation: field is mandatory
        public int ProductId { get; set; }
        
        /// <summary>
        /// Navigation property to the product in this cart item
        /// Virtual property for Entity Framework lazy loading
        /// Many-to-one relationship: many cart items can reference one product
        /// </summary>
        public virtual Product Product { get; set; } = null!;
        
        /// <summary>
        /// Quantity of the product in the cart
        /// Required field that must be at least 1 (cannot add 0 or negative items)
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]  // Validation: minimum 1
        public int Quantity { get; set; }
    }
}
