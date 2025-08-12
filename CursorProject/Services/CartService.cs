using CursorProject.Data;
using CursorProject.DTOs;
using CursorProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace CursorProject.Services
{
    public interface ICartService
    {
        Task<CartResponse> GetCartAsync(string userId);
        Task<CartResponse> AddToCartAsync(string userId, AddToCartRequest request);
        Task<CartResponse> UpdateCartItemAsync(string userId, int itemId, UpdateCartItemRequest request);
        Task<CartResponse> RemoveFromCartAsync(string userId, int itemId);
        Task<CartResponse> ClearCartAsync(string userId);
    }

    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartResponse> GetCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItems = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductImageUrl = ci.Product.ImageUrl,
                ProductPrice = ci.Product.Price,
                Quantity = ci.Quantity,
                TotalPrice = ci.Product.Price * ci.Quantity,
                AvailableStock = ci.Product.StockQuantity
            }).ToList();

            return new CartResponse
            {
                Success = true,
                CartItems = cartItems,
                TotalItems = cartItems.Sum(ci => ci.Quantity),
                TotalAmount = cartItems.Sum(ci => ci.TotalPrice)
            };
        }

        public async Task<CartResponse> AddToCartAsync(string userId, AddToCartRequest request)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                {
                    return new CartResponse
                    {
                        Success = false,
                        Message = "Product not found"
                    };
                }

                if (product.StockQuantity < request.Quantity)
                {
                    return new CartResponse
                    {
                        Success = false,
                        Message = "Insufficient stock"
                    };
                }

                cart.CartItems.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }

            await _context.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<CartResponse> UpdateCartItemAsync(string userId, int itemId, UpdateCartItemRequest request)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "Cart not found"
                };
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == itemId);
            if (cartItem == null)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "Cart item not found"
                };
            }

            if (cartItem.Product.StockQuantity < request.Quantity)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "Insufficient stock"
                };
            }

            cartItem.Quantity = request.Quantity;
            await _context.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<CartResponse> RemoveFromCartAsync(string userId, int itemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "Cart not found"
                };
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == itemId);
            if (cartItem == null)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "Cart item not found"
                };
            }

            cart.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<CartResponse> ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                cart.CartItems.Clear();
                await _context.SaveChangesAsync();
            }

            return new CartResponse
            {
                Success = true,
                Message = "Cart cleared successfully",
                CartItems = new List<CartItemDto>(),
                TotalItems = 0,
                TotalAmount = 0
            };
        }
    }
}
