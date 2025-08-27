using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetCartItemsByUserIdAsync(string userId);
        Task<CartItem> GetCartItemAsync(string userId, int productId);
        Task AddCartItemAsync(CartItem cartItem);
        void UpdateCartItem(CartItem cartItem);
        void RemoveCartItem(CartItem cartItem);
        Task<int> SaveChangesAsync();
    }
}
