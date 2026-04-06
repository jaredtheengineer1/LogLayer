using LogLayer.Data;
using LogLayer.Dtos;
using LogLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace LogLayer.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _db;

        public LogService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<LogEvent> CreateLogAsync(CreateLogRequest request, RequestContext context)
        {
            var log = new LogEvent
            {
                TenantId = new Guid("11111111-1111-1111-1111-111111111111"), // Placeholder tenant ID
                UserGuid = context.UserGuid,
                SessionGuid = context.SessionGuid,
                EventName = request.Event,
                Route = request.Route,
                Status = request.Status,
                Metadata = request.Metadata
            };

            _db.Logs.Add(log);
            await _db.SaveChangesAsync();

            return log;
        }

        public async Task<List<LogEvent>> GetLogsAsync(LogQueryParams query)
        {
            var logsQuery = _db.Logs.AsQueryable();

            var end = query.End ?? DateTime.UtcNow;
            var start = query.Start ?? end.AddHours(-12);

            logsQuery = logsQuery
              .Where(query => query.CreatedAt >= start && query.CreatedAt <= end);

            if (query.UserGuid.HasValue)
            {
                logsQuery = logsQuery.Where(log => log.UserGuid == query.UserGuid);
            }

            if (query.SessionGuid.HasValue)
            {
                logsQuery = logsQuery
                  .Where(logsQuery => logsQuery.SessionGuid == query.SessionGuid);
            }

            if (!string.IsNullOrWhiteSpace(query.EventName))
            {
                logsQuery = logsQuery
                  .Where(log => log.EventName == query.EventName);
            }

            if (!string.IsNullOrEmpty(query.Route))
            {
                logsQuery = logsQuery
                  .Where(log => log.Route == query.Route);
            }

            var limit = Math.Min(query.Limit, 500);
            var offset = Math.Max(query.Offset, 0);

            return await logsQuery
              .OrderByDescending(logsQuery => logsQuery.CreatedAt)
              .Skip(offset)
              .Take(query.Limit)
              .ToListAsync();
        }
    }
}