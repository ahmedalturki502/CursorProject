using System.ComponentModel.DataAnnotations;

namespace CursorProject.DTOs.Auth
{
    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;
    }
}
