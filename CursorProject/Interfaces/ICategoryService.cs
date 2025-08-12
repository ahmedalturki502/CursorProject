using CursorProject.DTOs.Category;
using CursorProject.DTOs;

namespace CursorProject.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, CreateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
