namespace CursorProject.DTOs.Cart
{
    public class CartResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
