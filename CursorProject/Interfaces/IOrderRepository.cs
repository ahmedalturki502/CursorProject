using CursorProject.Entities;

namespace CursorProject.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderWithItemsAsync(int orderId, string userId);
    }
}
