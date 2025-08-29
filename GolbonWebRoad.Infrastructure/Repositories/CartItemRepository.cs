using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace GolbonWebRoad.Infrastructure.Repositories
{

    public class CartItemRepository : ICartItemRepository
    {
        private readonly GolbonWebRoadDbContext _context;

        public CartItemRepository(GolbonWebRoadDbContext context)
        {
            _context=context;

        }
        public async Task<IEnumerable<CartItem>> GetCartItemsByUserIdAsync(string userId)
        {
            return await _context.CartItems
            .Where(ci => ci.UserId==userId)
            .Include(ci => ci.Product)
            .ToListAsync();

        }
        public async Task<CartItem> GetCartItemAsync(string userId, int productId)
        {

            return await _context.CartItems.
            FirstOrDefaultAsync(ci => ci.UserId==userId && ci.ProductId==productId);
        }
        public void AddCartItem(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);

        }
        public void UpdateCartItem(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);

        }
        public void RemoveCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
        }
        public void RemoveAllCartItem(IEnumerable<CartItem> cartItems)
        {
            _context.CartItems.RemoveRange(cartItems);
        }
        public async Task<int> SaveChangesAsync()
        {

            return await _context.SaveChangesAsync();
        }

    }

}