// Import necessary namespaces for the products controller
using CursorProject.DTOs.Product;              // Data transfer objects
using CursorProject.DTOs.Category;             // Category DTOs
using CursorProject.Services;                  // Custom services (ProductService)
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes

// Namespace for API controllers
namespace CursorProject.Controllers
{
    /// <summary>
    /// Controller for handling product-related operations
    /// Provides endpoints for browsing, creating, updating, and deleting products
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/products
    public class ProductsController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Product service for handling product operations
        /// </summary>
        private readonly IProductService _productService;

        /// <summary>
        /// Constructor that accepts product service via dependency injection
        /// </summary>
        /// <param name="productService">Product service for product operations</param>
        public ProductsController(IProductService productService)
        {
            _productService = productService;  // Store product service reference
        }

        /// <summary>
        /// Gets a paginated list of products with optional filtering and search
        /// GET: api/products
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of products per page (default: 10)</param>
        /// <param name="categoryId">Optional category filter</param>
        /// <param name="search">Optional search term for product name or description</param>
        /// <returns>Paginated list of products with metadata</returns>
        [HttpGet]  // HTTP GET endpoint
        public async Task<ActionResult<ProductListResponse>> GetProducts(
            [FromQuery] int page = 1,           // Page number from query string
            [FromQuery] int pageSize = 10,      // Page size from query string
            [FromQuery] int? categoryId = null, // Optional category filter
            [FromQuery] string? search = null)  // Optional search term
        {
            var response = await _productService.GetProductsAsync(page, pageSize, categoryId, search);
            return Ok(response);
        }

        /// <summary>
        /// Gets a specific product by its ID
        /// GET: api/products/{id}
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]  // HTTP GET endpoint with ID parameter
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();  // Return 404 if product not found
            }

            return Ok(product);
        }

        /// <summary>
        /// Creates a new product
        /// POST: api/products
        /// Requires admin authorization
        /// </summary>
        /// <param name="request">Product creation request</param>
        /// <returns>Created product details</returns>
        [HttpPost]  // HTTP POST endpoint
        [Authorize(Roles = "Admin")]  // Require admin role
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto request)
        {
            try
            {
                var product = await _productService.CreateProductAsync(request);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing product
        /// PUT: api/products/{id}
        /// Requires admin authorization
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="request">Product update request</param>
        /// <returns>Updated product details</returns>
        [HttpPut("{id}")]  // HTTP PUT endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Require admin role
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductRequest request)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, request);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a product
        /// DELETE: api/products/{id}
        /// Requires admin authorization
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]  // HTTP DELETE endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Require admin role
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound();  // Return 404 if product not found
            }

            return NoContent();  // Return 204 for successful deletion
        }

        /// <summary>
        /// Gets all available categories
        /// GET: api/products/categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet("categories")]  // HTTP GET endpoint for categories
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
