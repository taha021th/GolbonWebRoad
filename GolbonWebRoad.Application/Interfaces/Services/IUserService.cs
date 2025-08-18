using GolbonWebRoad.Application.Dtos.Users;

namespace GolbonWebRoad.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

    }
}
