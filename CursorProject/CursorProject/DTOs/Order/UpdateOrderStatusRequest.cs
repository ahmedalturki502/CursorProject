using CursorProject.Entities;
using System.ComponentModel.DataAnnotations;

namespace CursorProject.DTOs.Order
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
