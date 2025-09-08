using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Domain.Interfaces.Repositories;
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
        public IColorRepository ColorRepository { get; private set; }
        public IBrandRepository BrandRepository { get; private set; }

        public UnitOfWork(GolbonWebRoadDbContext context, ICartItemRepository cartItemRepository, IProductRepository productRepository, IOrderRepository orderRepository, ICategoryRepository categoryRepository, IColorRepository colorRepository, IBrandRepository brandRepository)
        {
            _context=context;
            CartItemRepository = cartItemRepository;
            ProductRepository=productRepository;
            OrderRepository=orderRepository;
            CategoryRepository= categoryRepository;
            ColorRepository= colorRepository;
            BrandRepository=brandRepository;
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
