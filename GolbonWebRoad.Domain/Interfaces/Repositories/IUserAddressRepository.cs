using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IUserAddressRepository
    {
        Task<UserAddress> AddAsync(UserAddress address);
        Task<UserAddress?> GetByIdAsync(int id);
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(string userId);
        void Update(UserAddress address);
        Task DeleteAsync(int id);
    }
}


