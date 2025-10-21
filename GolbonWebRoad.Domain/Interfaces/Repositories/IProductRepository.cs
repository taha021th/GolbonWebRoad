using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    /// <summary>
    /// کلاسی برای نگهداری نتایج صفحه‌بندی شده.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
    }

    /// <summary>
    /// اینترفیس (قرارداد) برای ریپازیتوری محصولات.
    /// تمام عملیات مربوط به خواندن و نوشتن اطلاعات محصولات از دیتابیس در اینجا تعریف می‌شود.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// یک محصول را با شناسه آن و با امکان بارگذاری اطلاعات مرتبط (join) برمی‌گرداند.
        /// </summary>
        Task<Product?> GetByIdAsync(int id, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrand = false);

        /// <summary>
        /// لیستی از تمام محصولات را با امکان فیلتر، مرتب‌سازی و محدود کردن تعداد برمی‌گرداند.
        /// </summary>
        Task<ICollection<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null, string? sortOrder = null, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrand = false, int count = 0);

        /// <summary>
        /// محصولات را به صورت صفحه‌بندی شده همراه با فیلتر و مرتب‌سازی برمی‌گرداند.
        /// </summary>
        Task<PagedResult<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null, int? categoryId = null, int? brandId = null, string sortOrder = null);

        /// <summary>
        /// محصولی که به عنوان "ویژه" علامت‌گذاری شده را برمی‌گرداند.
        /// </summary>
        Task<Product> GetProductByIsFeaturedAsync();

        /// <summary>
        /// یک محصول جدید به دیتابیس اضافه می‌کند.
        /// </summary>
        Product Add(Product product);

        /// <summary>
        /// اطلاعات یک محصول موجود را به‌روزرسانی می‌کند.
        /// </summary>
        void Update(Product product);

        /// <summary>
        /// یک محصول را با شناسه آن از دیتابیس حذف می‌کند.
        /// </summary>
        Task DeleteAsync(int id);

        // ==========================================================
        // === متدهای آماری برای داشبورد ===
        // ==========================================================

        /// <summary>
        /// تعداد کل محصولات موجود در سیستم را برمی‌گرداند.
        /// </summary>
        Task<int> GetTotalProductsCountAsync();

        /// <summary>
        /// تعداد محصولاتی که مجموع موجودی واریانت‌های آن‌ها کم است (کمتر از ۱۰) را برمی‌گرداند.
        /// </summary>
        Task<int> GetLowStockProductsCountAsync();
    }
}
