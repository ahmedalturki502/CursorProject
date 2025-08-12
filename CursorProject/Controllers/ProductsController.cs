using CursorProject.DTOs.Product;  // Import product data transfer objects
using CursorProject.DTOs.Category;  // Import category data transfer objects
using CursorProject.DTOs;  // Import main DTOs for responses and requests
using CursorProject.Services;  // Import business logic services
using Microsoft.AspNetCore.Authorization;  // Import authorization attributes and policies
using Microsoft.AspNetCore.Mvc;  // Import MVC controller base classes and attributes

namespace CursorProject.Controllers  // Define namespace for API controllers
{
    // Products controller that handles product catalog management endpoints
    // This controller provides REST API endpoints for product browsing, creation, updates, and deletion
    [ApiController]  // Indicates this is an API controller with automatic model validation
    [Route("api/[controller]")]  // Route template: api/products (controller name becomes route segment)
    public class ProductsController : ControllerBase  // Inherit from ControllerBase for API controller functionality
    {
        // Dependency injection field for product service
        private readonly IProductService _productService;  // Product service for handling product operations

        // Constructor for dependency injection of product service
        // This constructor is called by the DI container when creating the controller
        public ProductsController(IProductService productService)  // Inject product service
        {
            _productService = productService;  // Store product service reference
        }

        // Get paginated list of products with optional filtering
        // GET: api/products
        // This endpoint allows browsing products with search, category filtering, and pagination
        [HttpGet]  // HTTP GET endpoint for product listing
        public async Task<ActionResult<ProductListResponse>> GetProducts(  // Return paginated product list
            [FromQuery] int page = 1,  // Page number parameter with default value 1
            [FromQuery] int pageSize = 10,  // Page size parameter with default value 10
            [FromQuery] int? categoryId = null,  // Optional category filter parameter
            [FromQuery] string? search = null)  // Optional search term parameter
        {
            // Call product service to get paginated and filtered product list
            var response = await _productService.GetProductsAsync(page, pageSize, categoryId, search);  // Process product list request
            
            // Return success response with product list and pagination metadata
            return Ok(response);  // Return 200 OK with product list data
        }

        // Get a single product by its unique identifier
        // GET: api/products/{id}
        // This endpoint allows viewing detailed information about a specific product
        [HttpGet("{id}")]  // HTTP GET endpoint with route parameter for product ID
        public async Task<ActionResult<ProductDto>> GetProduct(int id)  // Accept product ID and return product details
        {
            // Call product service to get product by ID
            var product = await _productService.GetProductByIdAsync(id);  // Process product detail request
            
            // Return not found if product doesn't exist
            if (product == null)  // Check if product was found
                return NotFound();  // Return 404 Not Found for non-existent products
                
            // Return success response with product details
            return Ok(product);  // Return 200 OK with product information
        }

        // Create a new product in the system
        // POST: api/products
        // This endpoint requires admin authorization and allows creating new products
        [HttpPost]  // HTTP POST endpoint for product creation
        [Authorize(Roles = "Admin")]  // Require Admin role authorization
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto request)  // Accept product creation request
        {
            // Call product service to create new product
            var product = await _productService.CreateProductAsync(request);  // Process product creation request
            
            // Return success response with created product details
            // CreatedAtAction returns 201 Created with location header
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);  // Return 201 Created with product data
        }

        // Update an existing product in the system
        // PUT: api/products/{id}
        // This endpoint requires admin authorization and allows updating product information
        [HttpPut("{id}")]  // HTTP PUT endpoint with route parameter for product ID
        [Authorize(Roles = "Admin")]  // Require Admin role authorization
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductRequest request)  // Accept product ID and update request
        {
            try  // Try to update the product
            {
                // Call product service to update existing product
                var product = await _productService.UpdateProductAsync(id, request);  // Process product update request
                
                // Return success response with updated product details
                return Ok(product);  // Return 200 OK with updated product information
            }
            catch (ArgumentException ex)  // Catch argument exceptions (e.g., product not found)
            {
                // Return not found if product doesn't exist
                return NotFound(new { message = ex.Message });  // Return 404 Not Found with error message
            }
        }

        // Delete a product from the system
        // DELETE: api/products/{id}
        // This endpoint requires admin authorization and allows removing products
        [HttpDelete("{id}")]  // HTTP DELETE endpoint with route parameter for product ID
        [Authorize(Roles = "Admin")]  // Require Admin role authorization
        public async Task<ActionResult> DeleteProduct(int id)  // Accept product ID for deletion
        {
            // Call product service to delete product
            var success = await _productService.DeleteProductAsync(id);  // Process product deletion request
            
            // Return not found if product doesn't exist
            if (!success)  // Check if deletion was successful
                return NotFound();  // Return 404 Not Found for non-existent products
                
            // Return success response with no content
            return NoContent();  // Return 204 No Content for successful deletion
        }

        // Get all available product categories
        // GET: api/products/categories
        // This endpoint provides category list for product filtering
        [HttpGet("categories")]  // HTTP GET endpoint for category listing
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()  // Return list of all categories
        {
            // Call product service to get all categories
            var categories = await _productService.GetCategoriesAsync();  // Process category list request
            
            // Return success response with category list
            return Ok(categories);  // Return 200 OK with category list data
        }
    }
}
