namespace CursorProject.DTOs.Product
{
    public class ProductListResponse
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
