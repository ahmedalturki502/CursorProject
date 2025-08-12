using System.ComponentModel.DataAnnotations;

namespace CursorProject.DTOs.Category
{
    public class UpdateCategoryRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
    }
}
