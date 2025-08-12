// Import data validation attributes for property validation
using System.ComponentModel.DataAnnotations;

// Namespace for all application models
namespace CursorProject.Entities
{
    /// <summary>
    /// Represents a shopping cart for a user in the e-commerce system
    /// Contains items that the user has added but not yet purchased
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// Primary key for the cart
        /// Auto-generated unique identifier for each cart
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key reference to the user who owns this cart
        /// Links the cart to the customer who is shopping
        /// </summary>
        [Required]  // Validation: field is mandatory
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Navigation property to the user who owns this cart
        /// Virtual property for Entity Framework lazy loading
        /// One-to-one relationship: one user has one cart
        /// </summary>
        public virtual ApplicationUser User { get; set; } = null!;
        
        /// <summary>
        /// Date and time when the cart was created
        /// Auto-set to current UTC time when cart is created
        /// Used for cart management and cleanup of old carts
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Date and time when the cart was last updated
        /// Auto-updated whenever cart items are modified
        /// Used for tracking cart changes and session management
        /// </summary>
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        

        
        // Collection of items in this shopping cart
        // Virtual keyword enables Entity Framework lazy loading (items loaded only when accessed)
        // One-to-many relationship: one cart can contain many cart items
        // Initialized as empty list to avoid null reference exceptions
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
