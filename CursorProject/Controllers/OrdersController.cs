// Import necessary namespaces for the orders controller
using CursorProject.DTOs;                // Data transfer objects
using CursorProject.Services;                  // Custom services (OrderService)
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes
using System.Security.Claims;                  // User claims for authentication

// Namespace for API controllers
namespace CursorProject.Controllers
{
    /// <summary>
    /// Controller for handling order-related operations
    /// Provides endpoints for creating, viewing, and managing orders
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/orders
    [Authorize]  // Require authentication for all order operations
    public class OrdersController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Order service for handling order operations
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        /// Constructor that accepts order service via dependency injection
        /// </summary>
        /// <param name="orderService">Order service for order operations</param>
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;  // Store order service reference
        }

        /// <summary>
        /// Gets a paginated list of user's orders
        /// GET: api/orders
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of orders per page (default: 10)</param>
        /// <returns>Paginated list of orders</returns>
        [HttpGet]  // HTTP GET endpoint
        public async Task<ActionResult<OrderListResponse>> GetOrders(
            [FromQuery] int page = 1,           // Page number from query string
            [FromQuery] int pageSize = 10)      // Page size from query string
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _orderService.GetOrdersAsync(userId, page, pageSize);
            return Ok(response);
        }

        /// <summary>
        /// Gets a specific order by ID
        /// GET: api/orders/{id}
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]  // HTTP GET endpoint with ID parameter
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetOrderByIdAsync(id, userId);
            if (order == null)
            {
                return NotFound();  // Return 404 if order not found
            }

            return Ok(order);
        }

        /// <summary>
        /// Creates a new order from the user's cart
        /// POST: api/orders
        /// </summary>
        /// <param name="request">Order creation request</param>
        /// <returns>Created order details</returns>
        [HttpPost]  // HTTP POST endpoint
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(userId, request);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the status of an order (Admin only)
        /// PUT: api/orders/{id}/status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="request">Status update request</param>
        /// <returns>Updated order details</returns>
        [HttpPut("{id}/status")]  // HTTP PUT endpoint for status update
        [Authorize(Roles = "Admin")]  // Require admin role
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, request);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Cancels an order
        /// DELETE: api/orders/{id}
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]  // HTTP DELETE endpoint
        public async Task<ActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var success = await _orderService.CancelOrderAsync(id, userId);
                if (!success)
                {
                    return NotFound();  // Return 404 if order not found
                }

                return NoContent();  // Return 204 for successful cancellation
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
