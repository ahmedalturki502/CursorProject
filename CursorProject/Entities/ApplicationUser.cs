using Microsoft.AspNetCore.Identity;  // Import ASP.NET Core Identity framework for user management

namespace CursorProject.Entities  // Define namespace for all domain entities
{
    // Custom user entity that extends ASP.NET Core Identity's base user class
    // This class represents a user in the e-commerce system with additional properties
    public class ApplicationUser : IdentityUser  // Inherit from IdentityUser to get basic user properties (Id, Email, UserName, etc.)
    {
        // User's full name (first name + last name combined)
        // This is a custom property not provided by the base IdentityUser class
        // Initialized as empty string to avoid null reference exceptions
        public string FullName { get; set; } = string.Empty;
        
        // Collection of all orders placed by this user
        // Virtual keyword enables Entity Framework lazy loading (orders loaded only when accessed)
        // One-to-many relationship: one user can place multiple orders
        // Initialized as empty list to avoid null reference exceptions
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        
        // Shopping cart associated with this user
        // Virtual keyword enables Entity Framework lazy loading (cart loaded only when accessed)
        // One-to-one relationship: each user has exactly one shopping cart
        // Nullable (?) because a user might not have a cart initially (created on first item addition)
        public virtual Cart? Cart { get; set; }

        // JWT refresh token for secure token renewal without requiring user to log in again
        // Stored as string in database, nullable because user might not have an active refresh token
        // Used in conjunction with RefreshTokenExpiryTime for token security
        public string? RefreshToken { get; set; }

        // Expiration date and time for the refresh token
        // Nullable DateTime because user might not have an active refresh token
        // When this time is reached, the refresh token becomes invalid and user must log in again
        // Provides security by limiting how long refresh tokens remain valid
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
