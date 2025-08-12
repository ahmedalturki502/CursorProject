using CursorProject.Data;
using CursorProject.DTOs.Product;
using CursorProject.DTOs.Category;
using CursorProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace CursorProject.Services
{
    public interface IProductService
    {
        Task<ProductListResponse> GetProductsAsync(int page = 1, int pageSize = 10, int? categoryId = null, string? search = null);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto request);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);
        Task<List<CategoryDto>> GetCategoriesAsync();
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductListResponse> GetProductsAsync(int page = 1, int pageSize = 10, int? categoryId = null, string? search = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity > 0);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm) || 
                                       p.Description.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return new ProductListResponse
            {
                Products = products,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            };
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var createdProduct = await _context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == product.Id);

            return new ProductDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                ImageUrl = createdProduct.ImageUrl,
                StockQuantity = createdProduct.StockQuantity,
                CategoryId = createdProduct.CategoryId,
                CategoryName = createdProduct.Category.Name
            };
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new ArgumentException("Product not found");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();

            var updatedProduct = await _context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == id);

            return new ProductDto
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                ImageUrl = updatedProduct.ImageUrl,
                StockQuantity = updatedProduct.StockQuantity,
                CategoryId = updatedProduct.CategoryId,
                CategoryName = updatedProduct.Category.Name
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();
        }
    }
}
