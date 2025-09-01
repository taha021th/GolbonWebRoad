using GolbonWebRoad.Domain.Interfaces.Repositories;

namespace GolbonWebRoad.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        ICartItemRepository CartItemRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        Task<int> CompleteAsync();
    }
}
