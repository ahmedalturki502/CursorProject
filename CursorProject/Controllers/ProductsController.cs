// Import necessary namespaces for the products controller
using CursorProject.Data;                      // Database context
using CursorProject.DTOs;                      // Data transfer objects
using CursorProject.Models;                    // Application models
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes
using Microsoft.EntityFrameworkCore;         // Entity Framework Core

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
        /// Database context for accessing product data
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor that accepts database context via dependency injection
        /// </summary>
        /// <param name="context">Database context for data access</param>
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;  // Store database context reference
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
            // Start with base query including category information
            var query = _context.Products
                .Include(p => p.Category)       // Include category data for each product
                .Where(p => p.StockQuantity > 0); // Only show products with available stock

            // Apply category filter if specified
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);  // Filter by category ID
            }

            // Apply search filter if specified
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();  // Convert search term to lowercase for case-insensitive search
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm) ||    // Search in product name
                                       p.Description.ToLower().Contains(searchTerm)); // Search in product description
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            
            // Calculate total pages needed
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Get products for current page with pagination
            var products = await query
                .Skip((page - 1) * pageSize)    // Skip products from previous pages
                .Take(pageSize)                 // Take only products for current page
                .Select(p => new ProductDto     // Project to DTO for response
                {
                    Id = p.Id,                  // Product ID
                    Name = p.Name,              // Product name
                    Description = p.Description, // Product description
                    Price = p.Price,            // Product price
                    ImageUrl = p.ImageUrl,      // Product image URL
                    StockQuantity = p.StockQuantity, // Available stock
                    CategoryId = p.CategoryId,  // Category ID
                    CategoryName = p.Category.Name // Category name for display
                })
                .ToListAsync();                 // Execute query and get results

            // Return paginated response with metadata
            return Ok(new ProductListResponse
            {
                Products = products,            // List of products for current page
                TotalCount = totalCount,        // Total number of products matching criteria
                PageNumber = page,              // Current page number
                PageSize = pageSize,            // Number of products per page
                TotalPages = totalPages         // Total number of pages
            });
        }

        /// <summary>
        /// Gets a specific product by ID
        /// GET: api/products/{id}
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]  // HTTP GET endpoint with ID parameter
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            // Find product by ID including category information
            var product = await _context.Products
                .Include(p => p.Category)       // Include category data
                .FirstOrDefaultAsync(p => p.Id == id);  // Find product by ID

            // Return 404 if product not found
            if (product == null)
            {
                return NotFound();
            }

            // Return product details as DTO
            return Ok(new ProductDto
            {
                Id = product.Id,                // Product ID
                Name = product.Name,            // Product name
                Description = product.Description, // Product description
                Price = product.Price,          // Product price
                ImageUrl = product.ImageUrl,    // Product image URL
                StockQuantity = product.StockQuantity, // Available stock
                CategoryId = product.CategoryId, // Category ID
                CategoryName = product.Category.Name // Category name
            });
        }

        /// <summary>
        /// Creates a new product (Admin only)
        /// POST: api/products
        /// </summary>
        /// <param name="request">Product creation request</param>
        /// <returns>Created product details</returns>
        [HttpPost]  // HTTP POST endpoint
        [Authorize(Roles = "Admin")]  // Requires Admin role
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest request)
        {
            // Check if product name already exists (names must be unique)
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == request.Name);
            if (existingProduct != null)
            {
                return BadRequest(new { Message = "Product name already exists" });  // Return 400 if name taken
            }

            // Check if the specified category exists
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return BadRequest(new { Message = "Category does not exist" });  // Return 400 if category invalid
            }

            // Create new product entity
            var product = new Product
            {
                Name = request.Name,            // Product name
                Description = request.Description, // Product description
                Price = request.Price,          // Product price
                ImageUrl = request.ImageUrl,    // Product image URL
                StockQuantity = request.StockQuantity, // Initial stock quantity
                CategoryId = request.CategoryId // Category ID
            };

            // Add product to database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();  // Save changes to database

            // Return 201 Created with product details and location header
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ProductDto
            {
                Id = product.Id,                // Product ID
                Name = product.Name,            // Product name
                Description = product.Description, // Product description
                Price = product.Price,          // Product price
                ImageUrl = product.ImageUrl,    // Product image URL
                StockQuantity = product.StockQuantity, // Stock quantity
                CategoryId = product.CategoryId, // Category ID
                CategoryName = category.Name    // Category name
            });
        }

        /// <summary>
        /// Updates an existing product (Admin only)
        /// PUT: api/products/{id}
        /// </summary>
        /// <param name="id">Product ID to update</param>
        /// <param name="request">Product update request</param>
        /// <returns>Updated product details</returns>
        [HttpPut("{id}")]  // HTTP PUT endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Requires Admin role
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductRequest request)
        {
            // Find product by ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();  // Return 404 if product not found
            }

            // Check if product name already exists (excluding current product)
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == request.Name && p.Id != id);
            if (existingProduct != null)
            {
                return BadRequest(new { Message = "Product name already exists" });  // Return 400 if name taken
            }

            // Check if the specified category exists
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return BadRequest(new { Message = "Category does not exist" });  // Return 400 if category invalid
            }

            // Check if reducing stock below quantity already ordered
            var orderedQuantity = await _context.OrderItems
                .Where(oi => oi.ProductId == id)  // Filter by product ID
                .SumAsync(oi => oi.Quantity);     // Sum all ordered quantities

            if (request.StockQuantity < orderedQuantity)
            {
                return BadRequest(new { Message = "Cannot reduce stock below quantity already ordered" });  // Return 400 if stock too low
            }

            // Update product properties
            product.Name = request.Name;            // Update name
            product.Description = request.Description; // Update description
            product.Price = request.Price;          // Update price
            product.ImageUrl = request.ImageUrl;    // Update image URL
            product.StockQuantity = request.StockQuantity; // Update stock quantity
            product.CategoryId = request.CategoryId; // Update category ID

            // Save changes to database
            await _context.SaveChangesAsync();

            // Return updated product details
            return Ok(new ProductDto
            {
                Id = product.Id,                // Product ID
                Name = product.Name,            // Updated name
                Description = product.Description, // Updated description
                Price = product.Price,          // Updated price
                ImageUrl = product.ImageUrl,    // Updated image URL
                StockQuantity = product.StockQuantity, // Updated stock quantity
                CategoryId = product.CategoryId, // Updated category ID
                CategoryName = category.Name    // Category name
            });
        }

        /// <summary>
        /// Deletes a product (Admin only)
        /// DELETE: api/products/{id}
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]  // HTTP DELETE endpoint with ID parameter
        [Authorize(Roles = "Admin")]  // Requires Admin role
        public async Task<ActionResult> DeleteProduct(int id)
        {
            // Find product by ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();  // Return 404 if product not found
            }

            // Check if product is part of any existing order (prevent deletion)
            var hasOrders = await _context.OrderItems
                .AnyAsync(oi => oi.ProductId == id);  // Check if any order items reference this product
            if (hasOrders)
            {
                return BadRequest(new { Message = "Cannot delete product that is part of existing orders" });  // Return 400 if product has orders
            }

            // Remove product from database
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();  // Save changes to database

            return NoContent();  // Return 204 No Content on successful deletion
        }
    }
}
