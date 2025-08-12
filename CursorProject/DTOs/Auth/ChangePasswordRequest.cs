using System.ComponentModel.DataAnnotations;  // Import data validation attributes for property validation

namespace CursorProject.DTOs.Auth  // Define namespace for authentication data transfer objects
{
    // Data transfer object for password change requests
    // This class contains the information needed to change a user's password
    public class ChangePasswordRequest
    {
        // User's current password for verification
        // Required field that must not be empty for security
        // Used to verify the user's identity before allowing password change
        [Required(ErrorMessage = "Current password is required")]  // Validation attribute: field is mandatory
        [StringLength(100, ErrorMessage = "Current password cannot exceed 100 characters")]  // Validation attribute: maximum 100 characters
        public string CurrentPassword { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // User's new password for account security
        // Required field that must meet minimum security requirements
        // Minimum length of 6 characters for basic security
        [Required(ErrorMessage = "New password is required")]  // Validation attribute: field is mandatory
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long")]  // Validation attribute: 6-100 characters
        public string NewPassword { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
        
        // Confirmation of the new password to prevent typos
        // Required field that must match the new password exactly
        // Used for validation to ensure user typed new password correctly
        [Required(ErrorMessage = "Password confirmation is required")]  // Validation attribute: field is mandatory
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]  // Validation attribute: must match NewPassword property
        public string ConfirmNewPassword { get; set; } = string.Empty;  // Initialize as empty string to avoid null reference exceptions
    }
}
