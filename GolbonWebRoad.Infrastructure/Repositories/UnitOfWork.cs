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
        public IReviewsRepository ReviewsRepository { get; private set; }
        public IProductAttributeRepository ProductAttributeRepository { get; private set; }
        public IProductAttributeValueRepository ProductAttributeValueRepository { get; private set; }
        public IProductVariantRepository ProductVariantRepository { get; private set; }
        public IUserAddressRepository UserAddressRepository { get; private set; }
        public IFaqRepository FaqRepository { get; private set; }
        public IFaqCategoryRepository FaqCategoryRepository { get; private set; }

        public UnitOfWork(GolbonWebRoadDbContext context, ICartItemRepository cartItemRepository, IProductRepository productRepository, IOrderRepository orderRepository, ICategoryRepository categoryRepository, IColorRepository colorRepository, IBrandRepository brandRepository, IReviewsRepository reviewsRepository, IProductAttributeRepository productAttributeRepository, IProductAttributeValueRepository productAttributeValueRepository, IProductVariantRepository productVariantRepository, IUserAddressRepository userAddressRepository, IFaqRepository faqRepository, IFaqCategoryRepository faqCategoryRepository)
        {
            _context=context;
            CartItemRepository = cartItemRepository;
            ProductRepository=productRepository;
            OrderRepository=orderRepository;
            CategoryRepository= categoryRepository;
            ColorRepository= colorRepository;
            BrandRepository=brandRepository;
            ReviewsRepository=reviewsRepository;
            ProductAttributeRepository=productAttributeRepository;
            ProductAttributeValueRepository=productAttributeValueRepository;
            ProductVariantRepository=productVariantRepository;
            UserAddressRepository=userAddressRepository;
            FaqRepository = faqRepository;
            FaqCategoryRepository = faqCategoryRepository;
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
