using CursorProject.DTOs.Order;

namespace CursorProject.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId);
        Task<OrderDto> CreateOrderAsync(string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
    }
}
