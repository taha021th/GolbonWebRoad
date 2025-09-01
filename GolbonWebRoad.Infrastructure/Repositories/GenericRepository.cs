
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Domain.Interfaces.Specifications;
using GolbonWebRoad.Infrastructure.Persistence;
using GolbonWebRoad.Infrastructure.Specifications.Base;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly GolbonWebRoadDbContext _context;
        public GenericRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();

        }
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }
    }
}
