using CursorProject.Data;
using CursorProject.Entities;
using CursorProject.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CursorProject.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> GetProductCountAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .CountAsync();
        }
    }
}
