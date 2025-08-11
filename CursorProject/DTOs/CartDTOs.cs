// Import data validation attributes for input validation
using System.ComponentModel.DataAnnotations;

// Namespace for Data Transfer Objects (DTOs)
namespace CursorProject.DTOs
{
    /// <summary>
    /// Data transfer object for adding products to the shopping cart
    /// Contains the product and quantity to add
    /// </summary>
    public class AddToCartRequest
    {
        /// <summary>
        /// ID of the product to add to the cart
        /// Required field to identify which product to add
        /// </summary>
        [Required]  // Validation: field is mandatory
        public int ProductId { get; set; }

        /// <summary>
        /// Quantity of the product to add to the cart
        /// Required field that must be at least 1
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]  // Validation: minimum 1
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating cart item quantities
    /// Contains the new quantity for an existing cart item
    /// </summary>
    public class UpdateCartItemRequest
    {
        /// <summary>
        /// New quantity for the cart item
        /// Required field that must be at least 1
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]  // Validation: minimum 1
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Data transfer object for cart item information in responses
    /// Contains complete cart item details including product information
    /// </summary>
    public class CartItemDto
    {
        /// <summary>
        /// Unique identifier for the cart item
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the product in this cart item
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
        /// Current price of the product
        /// </summary>
        public decimal ProductPrice { get; set; }
        
        /// <summary>
        /// Quantity of this product in the cart
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Total price for this cart item (ProductPrice * Quantity)
        /// </summary>
        public decimal TotalPrice { get; set; }
        
        /// <summary>
        /// Current available stock for this product
        /// Used to prevent ordering more than available
        /// </summary>
        public int AvailableStock { get; set; }
    }

    /// <summary>
    /// Data transfer object for shopping cart information in responses
    /// Contains all cart items and summary information
    /// </summary>
    public class CartDto
    {
        /// <summary>
        /// Unique identifier for the shopping cart
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// List of all items in the shopping cart
        /// </summary>
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        
        /// <summary>
        /// Total amount for all items in the cart
        /// Calculated as sum of all item total prices
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// Total number of items in the cart
        /// Calculated as sum of all item quantities
        /// </summary>
        public int TotalItems { get; set; }
    }
}
