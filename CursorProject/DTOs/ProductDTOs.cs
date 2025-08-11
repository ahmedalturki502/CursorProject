// Import data validation attributes for input validation
using System.ComponentModel.DataAnnotations;

// Namespace for Data Transfer Objects (DTOs)
namespace CursorProject.DTOs
{
    /// <summary>
    /// Data transfer object for creating new products
    /// Contains all required information to create a product
    /// </summary>
    public class CreateProductRequest
    {
        /// <summary>
        /// Name of the product (e.g., "iPhone 15", "Nike Running Shoes")
        /// Required field with maximum length of 200 characters
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(200)]  // Validation: maximum 200 characters
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the product
        /// Optional field with maximum length of 1000 characters
        /// </summary>
        [StringLength(1000)]  // Validation: maximum 1000 characters
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Price of the product in decimal format for precise currency handling
        /// Required field that must be greater than 0
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]  // Validation: minimum 0.01
        public decimal Price { get; set; }

        /// <summary>
        /// URL to the product image for display purposes
        /// Optional field with maximum length of 500 characters
        /// </summary>
        [StringLength(500)]  // Validation: maximum 500 characters
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Initial stock quantity available for the product
        /// Required field that must be 0 or greater
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]  // Validation: minimum 0
        public int StockQuantity { get; set; }

        /// <summary>
        /// ID of the category this product belongs to
        /// Required field to organize products by category
        /// </summary>
        [Required]  // Validation: field is mandatory
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating existing products
    /// Contains all fields that can be modified for a product
    /// </summary>
    public class UpdateProductRequest
    {
        /// <summary>
        /// Updated name of the product
        /// Required field with maximum length of 200 characters
        /// </summary>
        [Required]  // Validation: field is mandatory
        [StringLength(200)]  // Validation: maximum 200 characters
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Updated description of the product
        /// Optional field with maximum length of 1000 characters
        /// </summary>
        [StringLength(1000)]  // Validation: maximum 1000 characters
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Updated price of the product
        /// Required field that must be greater than 0
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]  // Validation: minimum 0.01
        public decimal Price { get; set; }

        /// <summary>
        /// Updated URL to the product image
        /// Optional field with maximum length of 500 characters
        /// </summary>
        [StringLength(500)]  // Validation: maximum 500 characters
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Updated stock quantity for the product
        /// Required field that must be 0 or greater
        /// </summary>
        [Required]  // Validation: field is mandatory
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]  // Validation: minimum 0
        public int StockQuantity { get; set; }

        /// <summary>
        /// Updated category ID for the product
        /// Required field to organize products by category
        /// </summary>
        [Required]  // Validation: field is mandatory
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// Data transfer object for product information in responses
    /// Contains complete product details including category information
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Detailed description of the product
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Current price of the product
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// URL to the product image
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Current stock quantity available
        /// </summary>
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// ID of the category this product belongs to
        /// </summary>
        public int CategoryId { get; set; }
        
        /// <summary>
        /// Name of the category this product belongs to
        /// Used for display purposes in the frontend
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for paginated product list responses
    /// Contains products and pagination information
    /// </summary>
    public class ProductListResponse
    {
        /// <summary>
        /// List of products for the current page
        /// </summary>
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        
        /// <summary>
        /// Total number of products matching the search/filter criteria
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; set; }
        
        /// <summary>
        /// Number of products per page
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Total number of pages available
        /// Calculated as TotalCount / PageSize (rounded up)
        /// </summary>
        public int TotalPages { get; set; }
    }
}
