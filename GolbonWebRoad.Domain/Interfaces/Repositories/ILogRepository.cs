using GolbonWebRoad.Domain.Entities;
using System.Linq.Expressions;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task<(IEnumerable<Log> Logs, int TotalCount)> GetLogsAsync(Expression<Func<Log, bool>> filter, int page, int pageSize);
    }
}
