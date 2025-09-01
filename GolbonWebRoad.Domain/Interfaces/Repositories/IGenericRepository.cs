using GolbonWebRoad.Domain.Interfaces.Specifications;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);


    }
}
