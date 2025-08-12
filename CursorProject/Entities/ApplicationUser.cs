// Import ASP.NET Identity namespace for user management
using Microsoft.AspNetCore.Identity;

// Namespace for all application models
namespace CursorProject.Entities
{
    /// <summary>
    /// Custom user model that extends ASP.NET Identity's IdentityUser
    /// Provides additional properties and relationships for the e-commerce application
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Full name of the user (first name + last name)
        /// This is a custom property not included in the base IdentityUser
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Collection of orders placed by this user
        /// Virtual property for Entity Framework lazy loading
        /// One-to-many relationship: one user can have many orders
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        
        /// <summary>
        /// Shopping cart associated with this user
        /// Virtual property for Entity Framework lazy loading
        /// One-to-one relationship: one user has one cart
        /// Nullable because a user might not have a cart initially
        /// </summary>
        public virtual Cart? Cart { get; set; }

        /// <summary>
        /// Refresh token for JWT token renewal
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Expiration time for the refresh token
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
