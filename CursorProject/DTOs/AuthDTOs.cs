// Import data validation attributes for input validation
using System.ComponentModel.DataAnnotations;

// Namespace for Data Transfer Objects (DTOs)
namespace CursorProject.DTOs
{
    /// <summary>
    /// Data transfer object for user registration requests
    /// Contains all required information to create a new user account
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Full name of the user (first name + last name)
        /// Required field with length validation between 2 and 100 characters
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]  // Validation: 2-100 characters
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email address for the user account
        /// Required field with email format validation
        /// </summary>
        [Required]  // Validation: field is mandatory
        [EmailAddress(ErrorMessage = "Invalid email address")]  // Validation: must be valid email format
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password for the user account
        /// Required field with complex password requirements
        /// Must contain at least one number and one special character
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]  // Validation: minimum 6 characters
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{6,}$",   // Regex pattern for password complexity
            ErrorMessage = "Password must contain at least one number and one special character")]  // Custom error message
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation to ensure user typed password correctly
        /// Required field that must match the Password field
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Compare("Password", ErrorMessage = "Passwords do not match")]  // Validation: must match Password field
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for user login requests
    /// Contains credentials for user authentication
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email address for user identification
        /// Required field with email format validation
        /// </summary>
        [Required]  // Validation: field is mandatory
        [EmailAddress(ErrorMessage = "Invalid email address")]  // Validation: must be valid email format
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password for user authentication
        /// Required field (no format validation as it's already validated during registration)
        /// </summary>
        [Required]  // Validation: field is mandatory
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for authentication responses
    /// Contains authentication result, tokens, and user information
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Indicates whether the authentication operation was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// JWT access token for API authentication
        /// Used for subsequent API requests
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// Refresh token for obtaining new access tokens
        /// Used when the access token expires
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
        
        /// <summary>
        /// Expiration date and time of the access token
        /// Used by client to know when to refresh the token
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        
        /// <summary>
        /// User information object containing basic user details
        /// </summary>
        public UserDto User { get; set; } = null!;
        
        /// <summary>
        /// List of user roles for authorization purposes
        /// Used by client to determine user permissions
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
        
        /// <summary>
        /// Optional message providing additional information about the operation
        /// Can contain success messages or error details
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for user information
    /// Contains basic user details without sensitive information
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public string Id { get; set; } = string.Empty;
        
        /// <summary>
        /// Full name of the user (first name + last name)
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for user profile update requests
    /// Contains editable user information
    /// </summary>
    public class UpdateProfileRequest
    {
        /// <summary>
        /// Updated full name for the user
        /// Required field with length validation between 2 and 100 characters
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]  // Validation: 2-100 characters
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Updated email address for the user
        /// Required field with email format validation
        /// </summary>
        [Required]  // Validation: field is mandatory
        [EmailAddress(ErrorMessage = "Invalid email address")]  // Validation: must be valid email format
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for refresh token requests
    /// Contains the expired token and refresh token for token renewal
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// The expired JWT access token
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// The refresh token for obtaining a new access token
        /// </summary>
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for password change requests
    /// Contains current and new password for password updates
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Current password for verification
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password to replace the current one
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{6,}$",
            ErrorMessage = "Password must contain at least one number and one special character")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation of the new password
        /// </summary>
        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
