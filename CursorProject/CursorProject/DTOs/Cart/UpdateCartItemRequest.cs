using System.ComponentModel.DataAnnotations;

namespace CursorProject.DTOs.Cart
{
    public class UpdateCartItemRequest
    {
        [Required]
        public int CartItemId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
