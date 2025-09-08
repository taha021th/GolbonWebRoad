using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IColorRepository
    {
        Task<Color?> FindByNameAsync(string name);
        Task AddAsync(Color color);
    }
}
