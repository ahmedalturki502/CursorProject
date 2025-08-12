// Import necessary namespaces for the cart controller
using CursorProject.DTOs;                 // Data transfer objects
using CursorProject.Services;                  // Custom services (CartService)
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes
using System.Security.Claims;                  // User claims for authentication

// Namespace for API controllers
namespace CursorProject.Controllers
{
    /// <summary>
    /// Controller for handling shopping cart operations
    /// Provides endpoints for managing cart items and cart state
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/cart
    [Authorize]  // Require authentication for all cart operations
    public class CartController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Cart service for handling cart operations
        /// </summary>
        private readonly ICartService _cartService;

        /// <summary>
        /// Constructor that accepts cart service via dependency injection
        /// </summary>
        /// <param name="cartService">Cart service for cart operations</param>
        public CartController(ICartService cartService)
        {
            _cartService = cartService;  // Store cart service reference
        }

        /// <summary>
        /// Gets the current user's shopping cart
        /// GET: api/cart
        /// </summary>
        /// <returns>Cart contents with items and totals</returns>
        [HttpGet]  // HTTP GET endpoint
        public async Task<ActionResult<CartResponse>> GetCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _cartService.GetCartAsync(userId);
            return Ok(response);
        }

        /// <summary>
        /// Adds a product to the shopping cart
        /// POST: api/cart
        /// </summary>
        /// <param name="request">Add to cart request</param>
        /// <returns>Updated cart contents</returns>
        [HttpPost]  // HTTP POST endpoint
        public async Task<ActionResult<CartResponse>> AddToCart(AddToCartRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _cartService.AddToCartAsync(userId, request);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Updates the quantity of a cart item
        /// PUT: api/cart/{itemId}
        /// </summary>
        /// <param name="itemId">Cart item ID</param>
        /// <param name="request">Update cart item request</param>
        /// <returns>Updated cart contents</returns>
        [HttpPut("{itemId}")]  // HTTP PUT endpoint with item ID parameter
        public async Task<ActionResult<CartResponse>> UpdateCartItem(int itemId, UpdateCartItemRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _cartService.UpdateCartItemAsync(userId, itemId, request);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Removes an item from the shopping cart
        /// DELETE: api/cart/{itemId}
        /// </summary>
        /// <param name="itemId">Cart item ID</param>
        /// <returns>Updated cart contents</returns>
        [HttpDelete("{itemId}")]  // HTTP DELETE endpoint with item ID parameter
        public async Task<ActionResult<CartResponse>> RemoveFromCart(int itemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _cartService.RemoveFromCartAsync(userId, itemId);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Clears all items from the shopping cart
        /// DELETE: api/cart
        /// </summary>
        /// <returns>Empty cart response</returns>
        [HttpDelete]  // HTTP DELETE endpoint
        public async Task<ActionResult<CartResponse>> ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _cartService.ClearCartAsync(userId);
            return Ok(response);
        }
    }
}
