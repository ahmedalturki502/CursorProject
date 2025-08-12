// Import data validation attributes for input validation
using System.ComponentModel.DataAnnotations;
using CursorProject.Entities;

// Namespace for Data Transfer Objects (DTOs)
namespace CursorProject.DTOs
{
    /// <summary>
    /// Data transfer object for placing new orders
    /// Contains the shipping address for the order
    /// </summary>
    public class PlaceOrderRequest
    {
        /// <summary>
        /// Shipping address where the order will be delivered
        /// Required field with minimum length of 10 characters for meaningful addresses
        /// Maximum length of 500 characters for complete address information
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Shipping address must be at least 10 characters")]  // Validation: 10-500 characters
        public string ShippingAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for order item information in responses
    /// Contains complete order item details including product information
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// Unique identifier for the order item
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the product in this order item
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// Name of the product for display purposes
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        
        /// <summary>
        /// URL to the product image for display purposes
        /// </summary>
        public string ProductImageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Quantity of this product ordered
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Price of the product at the time of purchase
        /// Stored separately from current product price to handle price changes
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Total price for this order item (Price * Quantity)
        /// </summary>
        public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// Data transfer object for order information in responses
    /// Contains complete order details including all order items
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Unique identifier for the order
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Date and time when the order was placed
        /// </summary>
        public DateTime OrderDate { get; set; }
        
        /// <summary>
        /// Total amount for the entire order
        /// Calculated as sum of all order item total prices
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Current status of the order
        /// </summary>
        public OrderStatus Status { get; set; }
        
        /// <summary>
        /// Shipping address where the order was delivered
        /// </summary>
        public string ShippingAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// List of all items in the order
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        
        /// <summary>
        /// Total number of items in the order
        /// Calculated as sum of all item quantities
        /// </summary>
        public int TotalItems { get; set; }
    }

    /// <summary>
    /// Data transfer object for admin order information
    /// Extends OrderDto to include customer information for admin views
    /// </summary>
    public class AdminOrderDto : OrderDto
    {
        /// <summary>
        /// ID of the customer who placed the order
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;
        
        /// <summary>
        /// Full name of the customer who placed the order
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email address of the customer who placed the order
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for paginated order list responses
    /// Contains orders and pagination information
    /// </summary>
    public class OrderListResponse
    {
        /// <summary>
        /// List of orders for the current page
        /// </summary>
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        
        /// <summary>
        /// Total number of orders matching the search/filter criteria
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; set; }
        
        /// <summary>
        /// Number of orders per page
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Total number of pages available
        /// Calculated as TotalCount / PageSize (rounded up)
        /// </summary>
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating new orders
    /// Contains order creation information
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// Shipping address where the order will be delivered
        /// </summary>
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Shipping address must be at least 10 characters")]
        public string ShippingAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for updating order status
    /// Contains the new status for an order
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        /// <summary>
        /// New status for the order
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; }
    }
}
