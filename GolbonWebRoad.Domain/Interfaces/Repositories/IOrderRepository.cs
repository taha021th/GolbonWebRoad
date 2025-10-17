using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Order Add(Order order);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        void Update(Order order);
        // ==========================================================
        // === متد جدید برای خواندن سفارش به همراه جزئیات کامل ===
        // ==========================================================
        /// <summary>
        /// یک سفارش را به همراه تمام جزئیات مورد نیاز (آیتم‌ها و آدرس) برای پردازش‌های بعدی برمی‌گرداند.
        /// </summary>
        /// <param name="id">شناسه سفارش</param>
        /// <returns>موجودیت سفارش به همراه جزئیات</returns>
        Task<Order?> GetOrderWithDetailsAsync(int id);
    }
}
