using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)System.Math.Ceiling(TotalCount/(double)PageSize);
    }
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrand = false);
        Task<ICollection<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null, string? sortOrder = null, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrand = false, int count = 0);
        Task<PagedResult<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null, int? categoryId = null, int? brandId = null, string sortOrder = null);
        Task<Product> GetProductByIsFeaturedAsync();
        Product Add(Product product);
        void Update(Product product);
        Task DeleteAsync(int id);
        
        // ==========================================================
        // === متدهای آماری برای داشبورد ===
        // ==========================================================
        
        /// <summary>
        /// تعداد کل محصولات
        /// </summary>
        /// <returns>تعداد کل محصولات</returns>
        Task<int> GetTotalProductsCountAsync();

        /// <summary>
        /// تعداد محصولات کم موجود (کمتر از 10)
        /// </summary>
        /// <returns>تعداد محصولات کم موجود</returns>
        Task<int> GetLowStockProductsCountAsync();
    }
}
