using CursorProject.Data;
using CursorProject.Interfaces;

namespace CursorProject.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IProductRepository? _productRepository;
        private ICategoryRepository? _categoryRepository;
        private IOrderRepository? _orderRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
