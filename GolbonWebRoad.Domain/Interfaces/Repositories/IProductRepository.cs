using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrands = false);
        Task<ICollection<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null, string? sortOrder = null, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrands = false);
        Product Add(Product product);
        void Update(Product product);
        Task DeleteAsync(int id);
    }
}
