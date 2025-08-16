using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, bool? joinCategory = false);
        Task<ICollection<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null, string? sortOrder = null, bool? joinCategory = false);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
