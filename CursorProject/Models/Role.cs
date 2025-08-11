// Import ASP.NET Identity namespace for role management
using Microsoft.AspNetCore.Identity;

// Namespace for all application models
namespace CursorProject.Models
{
    /// <summary>
    /// Custom role model that extends ASP.NET Identity's IdentityRole
    /// Provides role-based authorization for the e-commerce application
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// Default constructor that calls the base IdentityRole constructor
        /// Creates a role without a name (name can be set later)
        /// </summary>
        public ApplicationRole() : base() { }
        
        /// <summary>
        /// Constructor that creates a role with a specific name
        /// Calls the base IdentityRole constructor with the provided role name
        /// </summary>
        /// <param name="roleName">The name of the role (e.g., "Admin", "Customer")</param>
        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
