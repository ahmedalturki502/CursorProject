namespace CursorProject.DTOs.Order
{
    public class OrderListResponse
    {
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
