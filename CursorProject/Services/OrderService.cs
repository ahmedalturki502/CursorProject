using CursorProject.Data;
using CursorProject.DTOs.Order;
using CursorProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace CursorProject.Services
{
    public interface IOrderService
    {
        Task<OrderListResponse> GetOrdersAsync(string userId, int page = 1, int pageSize = 10);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId);
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderRequest request);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);
        Task<bool> CancelOrderAsync(int orderId, string userId);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderListResponse> GetOrdersAsync(string userId, int page = 1, int pageSize = 10)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        ProductImageUrl = oi.Product.ImageUrl,
                        Price = oi.Price,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Price * oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return new OrderListResponse
            {
                Orders = orders,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImageUrl = oi.Product.ImageUrl,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Price * oi.Quantity
                }).ToList()
            };
        }

        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderRequest request)
        {
            // Get user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            // Validate stock availability
            foreach (var item in cart.CartItems)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product: {item.Product.Name}");
                }
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity)
            };

            _context.Orders.Add(order);

            // Create order items and update stock
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Price = cartItem.Product.Price,
                    Quantity = cartItem.Quantity
                };

                _context.OrderItems.Add(orderItem);

                // Update product stock
                cartItem.Product.StockQuantity -= cartItem.Quantity;
            }

            // Clear cart
            cart.CartItems.Clear();

            await _context.SaveChangesAsync();

            // Return the created order
            return await GetOrderByIdAsync(order.Id, userId) ?? throw new InvalidOperationException("Failed to create order");
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            order.Status = request.Status;
            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(orderId, order.UserId) ?? throw new InvalidOperationException("Failed to update order");
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be cancelled");
            }

            // Restore product stock
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Product.StockQuantity += orderItem.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
