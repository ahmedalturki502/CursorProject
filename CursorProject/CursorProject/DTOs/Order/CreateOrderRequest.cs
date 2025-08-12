using System.ComponentModel.DataAnnotations;

namespace CursorProject.DTOs.Order
{
    public class CreateOrderRequest
    {
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Shipping address must be at least 10 characters")]
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
