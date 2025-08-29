using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetCartItemsByUserIdAsync(string userId);
        Task<CartItem> GetCartItemAsync(string userId, int productId);
        Task AddCartItemAsync(CartItem cartItem);
        void UpdateCartItem(CartItem cartItem);
        void RemoveCartItem(CartItem cartItem);
        void RemoveAllCartItem(IEnumerable<CartItem> cartItems);
        Task<int> SaveChangesAsync();
    }
}
