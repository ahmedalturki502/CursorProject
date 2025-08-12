using System.ComponentModel.DataAnnotations;

namespace CursorProject.DTOs.Product
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
