using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Infrastructure.Persistence;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GolbonWebRoadDbContext _context;

        public ICartItemRepository CartItemRepository { get; private set; }

        public IProductRepository ProductRepository { get; private set; }

        public IOrderRepository OrderRepository { get; private set; }

        public ICategoryRepository CategoryRepository { get; private set; }

        public UnitOfWork(GolbonWebRoadDbContext context)
        {
            _context=context;
            CartItemRepository = new CartItemRepository(_context);
            ProductRepository=new ProductRepository(_context);
            OrderRepository=new OrderRepository(_context);
            CategoryRepository= new CategoryRepository(_context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }



        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
