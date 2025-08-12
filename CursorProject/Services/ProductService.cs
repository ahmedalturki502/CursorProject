using CursorProject.Data;  // Import database context for data access
using CursorProject.DTOs.Product;  // Import product data transfer objects
using CursorProject.DTOs.Category;  // Import category data transfer objects
using CursorProject.DTOs;  // Import main DTOs for responses and requests
using CursorProject.Entities;  // Import domain entities
using Microsoft.EntityFrameworkCore;  // Import Entity Framework Core for database operations

namespace CursorProject.Services  // Define namespace for business logic services
{
    // Interface defining the contract for product service operations
    // This interface allows for dependency injection and unit testing
    public interface IProductService
    {
        Task<ProductListResponse> GetProductsAsync(int page = 1, int pageSize = 10, int? categoryId = null, string? search = null);  // Get paginated list of products with optional filtering
        Task<ProductDto?> GetProductByIdAsync(int id);  // Get single product by its unique identifier
        Task<ProductDto> CreateProductAsync(CreateProductDto request);  // Create new product in the system
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request);  // Update existing product information
        Task<bool> DeleteProductAsync(int id);  // Delete product from the system
        Task<List<CategoryDto>> GetCategoriesAsync();  // Get all available product categories
    }

    // Implementation of product service that handles product catalog management
    // This class contains all business logic for product operations
    public class ProductService : IProductService  // Implement the IProductService interface
    {
        // Dependency injection field for database context
        private readonly ApplicationDbContext _context;  // Database context for Entity Framework operations

        // Constructor for dependency injection of database context
        public ProductService(ApplicationDbContext context)  // Inject database context
        {
            _context = context;  // Store database context reference
        }

        // Get paginated list of products with optional filtering by category and search term
        // This method handles product catalog browsing with search and filtering capabilities
        public async Task<ProductListResponse> GetProductsAsync(int page = 1, int pageSize = 10, int? categoryId = null, string? search = null)
        {
            // Start building the query to get products with their categories included
            var query = _context.Products  // Get products from database
                .Include(p => p.Category)  // Include category information in the query (eager loading)
                .Where(p => p.StockQuantity > 0);  // Filter to show only products with stock available

            // Apply category filter if category ID is provided
            if (categoryId.HasValue)  // Check if category filter is specified
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);  // Filter products by category ID
            }

            // Apply search filter if search term is provided
            if (!string.IsNullOrWhiteSpace(search))  // Check if search term is not empty
            {
                var searchTerm = search.ToLower();  // Convert search term to lowercase for case-insensitive search
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm) ||  // Search in product name
                                       p.Description.ToLower().Contains(searchTerm));  // Search in product description
            }

            // Get total count of products matching the filters for pagination
            var totalCount = await query.CountAsync();  // Count total matching products
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);  // Calculate total number of pages

            // Get the actual products for the current page with pagination
            var products = await query  // Apply the built query
                .Skip((page - 1) * pageSize)  // Skip products from previous pages
                .Take(pageSize)  // Take only products for current page
                .Select(p => new ProductDto  // Transform database entities to DTOs
                {
                    Id = p.Id,  // Set product ID
                    Name = p.Name,  // Set product name
                    Description = p.Description,  // Set product description
                    Price = p.Price,  // Set product price
                    ImageUrl = p.ImageUrl,  // Set product image URL
                    StockQuantity = p.StockQuantity,  // Set stock quantity
                    CategoryId = p.CategoryId,  // Set category ID
                    CategoryName = p.Category.Name  // Set category name
                })
                .ToListAsync();  // Execute query and return list

            // Return paginated response with products and pagination metadata
            return new ProductListResponse
            {
                Products = products,  // Set the list of products
                TotalCount = totalCount,  // Set total number of products
                PageNumber = page,  // Set current page number
                PageSize = pageSize,  // Set page size
                TotalPages = totalPages  // Set total number of pages
            };
        }

        // Get a single product by its unique identifier
        // This method handles product detail viewing
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            // Query the database to find product by ID with category information
            var product = await _context.Products  // Get products from database
                .Include(p => p.Category)  // Include category information (eager loading)
                .FirstOrDefaultAsync(p => p.Id == id);  // Find product with matching ID

            // Return null if product is not found
            if (product == null)  // Check if product exists
                return null;  // Return null for non-existent products

            // Transform database entity to DTO and return
            return new ProductDto
            {
                Id = product.Id,  // Set product ID
                Name = product.Name,  // Set product name
                Description = product.Description,  // Set product description
                Price = product.Price,  // Set product price
                ImageUrl = product.ImageUrl,  // Set product image URL
                StockQuantity = product.StockQuantity,  // Set stock quantity
                CategoryId = product.CategoryId,  // Set category ID
                CategoryName = product.Category.Name  // Set category name
            };
        }

        // Create a new product in the system
        // This method handles product creation with validation
        public async Task<ProductDto> CreateProductAsync(CreateProductDto request)
        {
            // Create new product entity from request data
            var product = new Product
            {
                Name = request.Name,  // Set product name from request
                Description = request.Description,  // Set product description from request
                Price = request.Price,  // Set product price from request
                ImageUrl = request.ImageUrl,  // Set product image URL from request
                StockQuantity = request.StockQuantity,  // Set stock quantity from request
                CategoryId = request.CategoryId  // Set category ID from request
            };

            // Add product to database context
            _context.Products.Add(product);  // Mark product for insertion
            await _context.SaveChangesAsync();  // Save changes to database

            // Load category information for the created product
            await _context.Entry(product)  // Get entity entry for the product
                .Reference(p => p.Category)  // Reference the category navigation property
                .LoadAsync();  // Load category data from database

            // Transform created entity to DTO and return
            return new ProductDto
            {
                Id = product.Id,  // Set product ID (auto-generated)
                Name = product.Name,  // Set product name
                Description = product.Description,  // Set product description
                Price = product.Price,  // Set product price
                ImageUrl = product.ImageUrl,  // Set product image URL
                StockQuantity = product.StockQuantity,  // Set stock quantity
                CategoryId = product.CategoryId,  // Set category ID
                CategoryName = product.Category.Name  // Set category name
            };
        }

        // Update an existing product in the system
        // This method handles product information updates
        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            // Find existing product by ID
            var product = await _context.Products  // Get products from database
                .Include(p => p.Category)  // Include category information
                .FirstOrDefaultAsync(p => p.Id == id);  // Find product with matching ID

            // Throw exception if product is not found
            if (product == null)  // Check if product exists
                throw new ArgumentException("Product not found");  // Throw exception for non-existent products

            // Update product properties with new values from request
            product.Name = request.Name;  // Update product name
            product.Description = request.Description;  // Update product description
            product.Price = request.Price;  // Update product price
            product.ImageUrl = request.ImageUrl;  // Update product image URL
            product.StockQuantity = request.StockQuantity;  // Update stock quantity
            product.CategoryId = request.CategoryId;  // Update category ID

            // Save changes to database
            await _context.SaveChangesAsync();  // Persist changes to database

            // Transform updated entity to DTO and return
            return new ProductDto
            {
                Id = product.Id,  // Set product ID
                Name = product.Name,  // Set updated product name
                Description = product.Description,  // Set updated product description
                Price = product.Price,  // Set updated product price
                ImageUrl = product.ImageUrl,  // Set updated product image URL
                StockQuantity = product.StockQuantity,  // Set updated stock quantity
                CategoryId = product.CategoryId,  // Set updated category ID
                CategoryName = product.Category.Name  // Set category name
            };
        }

        // Delete a product from the system
        // This method handles product removal with validation
        public async Task<bool> DeleteProductAsync(int id)
        {
            // Find product by ID
            var product = await _context.Products  // Get products from database
                .FirstOrDefaultAsync(p => p.Id == id);  // Find product with matching ID

            // Return false if product is not found
            if (product == null)  // Check if product exists
                return false;  // Return false for non-existent products

            // Remove product from database context
            _context.Products.Remove(product);  // Mark product for deletion
            await _context.SaveChangesAsync();  // Save changes to database

            return true;  // Return true for successful deletion
        }

        // Get all available product categories
        // This method provides category list for product filtering
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            // Query all categories from database and transform to DTOs
            return await _context.Categories  // Get categories from database
                .Select(c => new CategoryDto  // Transform to DTOs
                {
                    Id = c.Id,  // Set category ID
                    Name = c.Name,  // Set category name
                    Description = c.Description  // Set category description
                })
                .ToListAsync();  // Execute query and return list
        }
    }
}
