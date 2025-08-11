// Import necessary namespaces for the cart controller
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
    /// Controller for handling shopping cart operations
    /// Provides endpoints for managing user shopping carts
    /// All endpoints require authentication
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/cart
    [Authorize]  // Requires authentication for all endpoints
    public class CartController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Database context for accessing cart data
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor that accepts database context via dependency injection
        /// </summary>
        /// <param name="context">Database context for data access</param>
        public CartController(ApplicationDbContext context)
        {
            _context = context;  // Store database context reference
        }

        /// <summary>
        /// Gets the current user's shopping cart
        /// GET: api/cart
        /// Creates a new cart if one doesn't exist
        /// </summary>
        /// <returns>Shopping cart with all items and totals</returns>
        [HttpGet]  // HTTP GET endpoint
        public async Task<ActionResult<CartDto>> GetCart()
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

            // Create new cart if one doesn't exist
            if (cart == null)
            {
                cart = new Cart { UserId = userId };  // Create new cart for user
                _context.Carts.Add(cart);              // Add to database
                await _context.SaveChangesAsync();     // Save changes
            }

            // Convert cart items to DTOs with calculated totals
            var cartItems = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,                     // Cart item ID
                ProductId = ci.ProductId,       // Product ID
                ProductName = ci.Product.Name,  // Product name
                ProductImageUrl = ci.Product.ImageUrl, // Product image URL
                ProductPrice = ci.Product.Price, // Current product price
                Quantity = ci.Quantity,         // Quantity in cart
                TotalPrice = ci.Product.Price * ci.Quantity, // Calculated total price
                AvailableStock = ci.Product.StockQuantity // Available stock for validation
            }).ToList();

            // Return cart with calculated totals
            return Ok(new CartDto
            {
                Id = cart.Id,                           // Cart ID
                Items = cartItems,                      // List of cart items
                TotalAmount = cartItems.Sum(ci => ci.TotalPrice), // Sum of all item totals
                TotalItems = cartItems.Sum(ci => ci.Quantity)     // Sum of all quantities
            });
        }

        /// <summary>
        /// Adds a product to the user's shopping cart
        /// POST: api/cart/add
        /// </summary>
        /// <param name="request">Add to cart request with product ID and quantity</param>
        /// <returns>Updated shopping cart</returns>
        [HttpPost("add")]  // HTTP POST endpoint
        public async Task<ActionResult<CartDto>> AddToCart(AddToCartRequest request)
        {
            // Extract user ID from JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Check if product exists and has sufficient stock
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return BadRequest(new { Message = "Product does not exist" });  // Return 400 if product not found
            }

            // Validate stock availability
            if (request.Quantity > product.StockQuantity)
            {
                return BadRequest(new { Message = "Insufficient stock available" });  // Return 400 if insufficient stock
            }

            // Get or create user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)     // Include existing cart items
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Create new cart if one doesn't exist
            if (cart == null)
            {
                cart = new Cart { UserId = userId };  // Create new cart
                _context.Carts.Add(cart);              // Add to database
                await _context.SaveChangesAsync();     // Save to get cart ID
            }

            // Check if item already exists in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                // Update quantity for existing item
                var newQuantity = existingItem.Quantity + request.Quantity;  // Add new quantity to existing
                if (newQuantity > product.StockQuantity)
                {
                    return BadRequest(new { Message = "Insufficient stock available" });  // Return 400 if total exceeds stock
                }
                existingItem.Quantity = newQuantity;  // Update quantity
            }
            else
            {
                // Add new cart item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,           // Link to cart
                    ProductId = request.ProductId, // Product to add
                    Quantity = request.Quantity   // Quantity to add
                };
                _context.CartItems.Add(cartItem);  // Add to database
            }

            await _context.SaveChangesAsync();  // Save all changes

            return await GetCart();  // Return updated cart
        }

        /// <summary>
        /// Updates the quantity of an item in the user's shopping cart
        /// PUT: api/cart/items/{itemId}
        /// </summary>
        /// <param name="itemId">Cart item ID to update</param>
        /// <param name="request">Update request with new quantity</param>
        /// <returns>Updated shopping cart</returns>
        [HttpPut("items/{itemId}")]  // HTTP PUT endpoint with item ID
        public async Task<ActionResult<CartDto>> UpdateCartItem(int itemId, UpdateCartItemRequest request)
        {
            // Extract user ID from JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Find cart item and verify it belongs to the current user
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)         // Include cart for user verification
                .Include(ci => ci.Product)      // Include product for stock validation
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();  // Return 404 if item not found or doesn't belong to user
            }

            // Check if new quantity exceeds available stock
            if (request.Quantity > cartItem.Product.StockQuantity)
            {
                return BadRequest(new { Message = "Insufficient stock available" });  // Return 400 if insufficient stock
            }

            cartItem.Quantity = request.Quantity;  // Update quantity
            await _context.SaveChangesAsync();     // Save changes

            return await GetCart();  // Return updated cart
        }

        /// <summary>
        /// Removes an item from the user's shopping cart
        /// DELETE: api/cart/items/{itemId}
        /// </summary>
        /// <param name="itemId">Cart item ID to remove</param>
        /// <returns>Updated shopping cart</returns>
        [HttpDelete("items/{itemId}")]  // HTTP DELETE endpoint with item ID
        public async Task<ActionResult<CartDto>> RemoveCartItem(int itemId)
        {
            // Extract user ID from JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Find cart item and verify it belongs to the current user
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)         // Include cart for user verification
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();  // Return 404 if item not found or doesn't belong to user
            }

            _context.CartItems.Remove(cartItem);  // Remove item from database
            await _context.SaveChangesAsync();     // Save changes

            return await GetCart();  // Return updated cart
        }

        /// <summary>
        /// Clears all items from the user's shopping cart
        /// DELETE: api/cart/clear
        /// </summary>
        /// <returns>Empty shopping cart</returns>
        [HttpDelete("clear")]  // HTTP DELETE endpoint
        public async Task<ActionResult<CartDto>> ClearCart()
        {
            // Extract user ID from JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Get user's cart with all items
            var cart = await _context.Carts
                .Include(c => c.CartItems)     // Include all cart items
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Remove all cart items if cart exists
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);  // Remove all items
                await _context.SaveChangesAsync();               // Save changes
            }

            return await GetCart();  // Return updated (empty) cart
        }
    }
}
