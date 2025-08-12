using CursorProject.Entities;

namespace CursorProject.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<int> GetProductCountAsync(int categoryId);
    }
}
