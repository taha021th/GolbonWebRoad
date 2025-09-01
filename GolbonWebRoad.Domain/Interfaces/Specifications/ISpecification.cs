using System.Linq.Expressions;

namespace GolbonWebRoad.Domain.Interfaces.Specifications
{
    public interface ISpecification<T>
    {
        // معیارها یا شرط‌های فیلتر (مثلاً: l => l.Level == "Error")
        Expression<Func<T, bool>> Criteria { get; }
        // لیست جداولی که باید Join شوند (مثلاً برای Include کردن Category در Product)
        List<Expression<Func<T, object>>> Includes { get; }
        // معیار مرتب‌سازی صعودی
        Expression<Func<T, object>> OrderBy { get; }
        // معیار مرتب‌سازی نزولی
        Expression<Func<T, object>> OrderByDescending { get; }
    }
}
