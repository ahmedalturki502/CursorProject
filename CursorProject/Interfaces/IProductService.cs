using CursorProject.DTOs.Product;

namespace CursorProject.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id, CreateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
    }
}
