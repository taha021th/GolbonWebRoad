using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public LogRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }
        public async Task<(IEnumerable<Log> Logs, int TotalCount)> GetLogsAsync(Expression<Func<Log, bool>> filter, int page, int pageSize)
        {
            var queryTest = await _context.Logs.ToListAsync();
            var query = _context.Logs.Where(filter);
            var totalCount = await query.CountAsync();
            var logs = await query
                .OrderByDescending(l => l.TimeStamp)
                .Skip((page-1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (logs, totalCount);
        }
    }
}
