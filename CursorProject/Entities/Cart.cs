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
        /// Collection of items in this shopping cart
        /// Virtual property for Entity Framework lazy loading
        /// One-to-many relationship: one cart can have many cart items
        /// </summary>
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
