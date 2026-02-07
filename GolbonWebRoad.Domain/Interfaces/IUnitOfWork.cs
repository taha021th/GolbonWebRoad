using GolbonWebRoad.Domain.Interfaces.Repositories;

namespace GolbonWebRoad.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        ICartItemRepository CartItemRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IBrandRepository BrandRepository { get; }
        IColorRepository ColorRepository { get; }
        IReviewsRepository ReviewsRepository { get; }
        IProductAttributeRepository ProductAttributeRepository { get; }
        IProductAttributeValueRepository ProductAttributeValueRepository { get; }
        IProductVariantRepository ProductVariantRepository { get; }
        IUserAddressRepository UserAddressRepository { get; }
        IFaqRepository FaqRepository { get; }
        IFaqCategoryRepository FaqCategoryRepository { get; }
        IBlogRepository BlogRepository { get; }
        IBlogCategoryRepository BlogCategoryRepository { get; }
        IBlogReviewRepository BlogReviewRepository { get; }
        Task<int> CompleteAsync();


    }

}

