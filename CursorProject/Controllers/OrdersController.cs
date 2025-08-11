// Import necessary namespaces for the orders controller
using CursorProject.Data;                      // Database context
using CursorProject.DTOs;                      // Data transfer objects
using CursorProject.Models;                    // Application models
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes
using Microsoft.EntityFrameworkCore;         // Entity Framework Core
using System.Security.Claims;                  // User claims for authentication

// Namespace for API controllers
namespace CursorProject.Controllers
{
    /// <summary>
    /// Controller for handling order-related operations
    /// Provides endpoints for placing orders and viewing order history
    /// All endpoints require authentication
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/orders
    [Authorize]  // Requires authentication for all endpoints
    public class OrdersController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Database context for accessing order data
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor that accepts database context via dependency injection
        /// </summary>
        /// <param name="context">Database context for data access</param>
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;  // Store database context reference
        }

        /// <summary>
        /// Places a new order from the user's shopping cart
        /// POST: api/orders
        /// </summary>
        /// <param name="request">Order placement request with shipping address</param>
        /// <returns>Created order details</returns>
        [HttpPost]  // HTTP POST endpoint
        public async Task<ActionResult<OrderDto>> PlaceOrder(PlaceOrderRequest request)
        {
            // Extract user ID from JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Get user's cart with all items and product information
            var cart = await _context.Carts
                .Include(c => c.CartItems)     // Include cart items
                .ThenInclude(ci => ci.Product) // Include product information for each item
                .FirstOrDefaultAsync(c => c.UserId == userId);  // Find cart by user ID

            // Validate cart is not empty
            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest(new { Message = "Cart is empty" });  // Return 400 if cart is empty
            }

            // Validate stock availability and calculate total amount
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            // Process each cart item
            foreach (var cartItem in cart.CartItems)
            {
                // Check if requested quantity exceeds available stock
                if (cartItem.Quantity > cartItem.Product.StockQuantity)
                {
                    return BadRequest(new { Message = $"Insufficient stock for {cartItem.Product.Name}" });  // Return 400 if insufficient stock
                }

                // Calculate total amount for this item
                totalAmount += cartItem.Product.Price * cartItem.Quantity;

                // Create order item (without OrderId for now)
                orderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,     // Product ID
                    Quantity = cartItem.Quantity,       // Quantity ordered
                    Price = cartItem.Product.Price      // Price at time of order
                });
            }

            // Create the order
            var order = new Order
            {
                UserId = userId,                        // User who placed the order
                OrderDate = DateTime.UtcNow,            // Current UTC time
                TotalAmount = totalAmount,              // Calculated total amount
                ShippingAddress = request.ShippingAddress // Shipping address from request
            };

            // Add order to database and get the generated ID
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();  // Save to get order ID

            // Set OrderId for all order items
            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;  // Link items to the order
            }
            _context.OrderItems.AddRange(orderItems);  // Add all order items

            // Update product stock quantities
            foreach (var cartItem in cart.CartItems)
            {
                cartItem.Product.StockQuantity -= cartItem.Quantity;  // Reduce stock by ordered quantity
            }

            // Clear the cart after successful order placement
            _context.CartItems.RemoveRange(cart.CartItems);  // Remove all cart items

            // Save all changes to database
            await _context.SaveChangesAsync();

            // Return the created order details
            return await GetOrder(order.Id);  // Return order details
        }

        /// <summary>
        /// Gets paginated list of orders for the current user
        /// GET: api/orders
        /// Customers see only their orders, admins see all orders
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of orders per page (default: 10)</param>
        /// <returns>Paginated list of orders</returns>
        [HttpGet]  // HTTP GET endpoint
        public async Task<ActionResult<OrderListResponse>> GetOrders(
            [FromQuery] int page = 1,           // Page number from query string
            [FromQuery] int pageSize = 10)      // Page size from query string
        {
            // Extract user ID from JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Check if user is admin for authorization
            var isAdmin = User.IsInRole("Admin");
            var query = _context.Orders.AsQueryable();

            // Customers can only see their own orders, admins can see all
            if (!isAdmin)
            {
                query = query.Where(o => o.UserId == userId);  // Filter by user ID for customers
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            
            // Calculate total pages needed
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Get orders for current page with all related data
            var orders = await query
                .Include(o => o.OrderItems)     // Include order items
                .ThenInclude(oi => oi.Product)  // Include product information
                .Include(o => o.User)           // Include user information
                .OrderByDescending(o => o.OrderDate)  // Sort by order date (newest first)
                .Skip((page - 1) * pageSize)    // Skip orders from previous pages
                .Take(pageSize)                 // Take only orders for current page
                .Select(o => new OrderDto       // Project to DTO for response
                {
                    Id = o.Id,                  // Order ID
                    OrderDate = o.OrderDate,    // Order date
                    TotalAmount = o.TotalAmount, // Total amount
                    ShippingAddress = o.ShippingAddress, // Shipping address
                    Items = o.OrderItems.Select(oi => new OrderItemDto  // Convert order items to DTOs
                    {
                        Id = oi.Id,             // Order item ID
                        ProductId = oi.ProductId, // Product ID
                        ProductName = oi.Product.Name, // Product name
                        ProductImageUrl = oi.Product.ImageUrl, // Product image URL
                        Quantity = oi.Quantity, // Quantity ordered
                        Price = oi.Price,       // Price at time of order
                        TotalPrice = oi.Price * oi.Quantity // Calculated total for this item
                    }).ToList(),
                    TotalItems = o.OrderItems.Sum(oi => oi.Quantity) // Total number of items in order
                })
                .ToListAsync();                 // Execute query and get results

            // Return paginated response with metadata
            return Ok(new OrderListResponse
            {
                Orders = orders,                // List of orders for current page
                TotalCount = totalCount,        // Total number of orders matching criteria
                PageNumber = page,              // Current page number
                PageSize = pageSize,            // Number of orders per page
                TotalPages = totalPages         // Total number of pages
            });
        }

        /// <summary>
        /// Gets a specific order by ID
        /// GET: api/orders/{id}
        /// Customers can only see their own orders, admins can see any order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]  // HTTP GET endpoint with ID parameter
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            // Extract user ID from JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Check if user is admin for authorization
            var isAdmin = User.IsInRole("Admin");

            // Find order by ID with all related data
            var order = await _context.Orders
                .Include(o => o.OrderItems)     // Include order items
                .ThenInclude(oi => oi.Product)  // Include product information
                .Include(o => o.User)           // Include user information
                .FirstOrDefaultAsync(o => o.Id == id);  // Find order by ID

            // Return 404 if order not found
            if (order == null)
            {
                return NotFound();
            }

            // Customers can only see their own orders
            if (!isAdmin && order.UserId != userId)
            {
                return Forbid();  // Return 403 if customer tries to access another user's order
            }

            // Return order details as DTO
            return Ok(new OrderDto
            {
                Id = order.Id,                  // Order ID
                OrderDate = order.OrderDate,    // Order date
                TotalAmount = order.TotalAmount, // Total amount
                ShippingAddress = order.ShippingAddress, // Shipping address
                Items = order.OrderItems.Select(oi => new OrderItemDto  // Convert order items to DTOs
                {
                    Id = oi.Id,                 // Order item ID
                    ProductId = oi.ProductId,   // Product ID
                    ProductName = oi.Product.Name, // Product name
                    ProductImageUrl = oi.Product.ImageUrl, // Product image URL
                    Quantity = oi.Quantity,     // Quantity ordered
                    Price = oi.Price,           // Price at time of order
                    TotalPrice = oi.Price * oi.Quantity // Calculated total for this item
                }).ToList(),
                TotalItems = order.OrderItems.Sum(oi => oi.Quantity) // Total number of items in order
            });
        }

        /// <summary>
        /// Gets all orders with filtering options (Admin only)
        /// GET: api/orders/admin/all
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of orders per page (default: 10)</param>
        /// <param name="customerEmail">Filter by customer email</param>
        /// <param name="fromDate">Filter orders from this date</param>
        /// <param name="toDate">Filter orders to this date</param>
        /// <returns>Paginated list of all orders with customer information</returns>
        [HttpGet("admin/all")]  // HTTP GET endpoint for admin
        [Authorize(Roles = "Admin")]  // Requires Admin role
        public async Task<ActionResult<OrderListResponse>> GetAllOrders(
            [FromQuery] int page = 1,           // Page number from query string
            [FromQuery] int pageSize = 10,      // Page size from query string
            [FromQuery] string? customerEmail = null,  // Optional customer email filter
            [FromQuery] DateTime? fromDate = null,     // Optional start date filter
            [FromQuery] DateTime? toDate = null)       // Optional end date filter
        {
            // Start with base query including all related data
            var query = _context.Orders
                .Include(o => o.OrderItems)     // Include order items
                .ThenInclude(oi => oi.Product)  // Include product information
                .Include(o => o.User)           // Include user information
                .AsQueryable();

            // Apply customer email filter if provided
            if (!string.IsNullOrWhiteSpace(customerEmail))
            {
                query = query.Where(o => o.User.Email!.Contains(customerEmail));  // Filter by customer email
            }

            // Apply start date filter if provided
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);  // Filter orders from this date
            }

            // Apply end date filter if provided
            if (toDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= toDate.Value);  // Filter orders to this date
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            
            // Calculate total pages needed
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Get orders for current page with admin-specific DTO
            var orders = await query
                .OrderByDescending(o => o.OrderDate)  // Sort by order date (newest first)
                .Skip((page - 1) * pageSize)    // Skip orders from previous pages
                .Take(pageSize)                 // Take only orders for current page
                .Select(o => new AdminOrderDto   // Project to admin DTO for response
                {
                    Id = o.Id,                  // Order ID
                    OrderDate = o.OrderDate,    // Order date
                    TotalAmount = o.TotalAmount, // Total amount
                    ShippingAddress = o.ShippingAddress, // Shipping address
                    CustomerId = o.UserId,      // Customer ID
                    CustomerName = o.User.FullName, // Customer full name
                    CustomerEmail = o.User.Email!, // Customer email
                    Items = o.OrderItems.Select(oi => new OrderItemDto  // Convert order items to DTOs
                    {
                        Id = oi.Id,             // Order item ID
                        ProductId = oi.ProductId, // Product ID
                        ProductName = oi.Product.Name, // Product name
                        ProductImageUrl = oi.Product.ImageUrl, // Product image URL
                        Quantity = oi.Quantity, // Quantity ordered
                        Price = oi.Price,       // Price at time of order
                        TotalPrice = oi.Price * oi.Quantity // Calculated total for this item
                    }).ToList(),
                    TotalItems = o.OrderItems.Sum(oi => oi.Quantity) // Total number of items in order
                })
                .ToListAsync();                 // Execute query and get results

            // Return paginated response with metadata
            return Ok(new OrderListResponse
            {
                Orders = orders.Cast<OrderDto>().ToList(), // Cast admin DTOs to base DTOs
                TotalCount = totalCount,        // Total number of orders matching criteria
                PageNumber = page,              // Current page number
                PageSize = pageSize,            // Number of orders per page
                TotalPages = totalPages         // Total number of pages
            });
        }
    }
}
