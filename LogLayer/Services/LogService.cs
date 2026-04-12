using LogLayer.Data;
using LogLayer.Dtos;
using LogLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
            Console.WriteLine($"creating: {request.EventName}, {request.Route}, {request.Status}");
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }
            var log = new LogEvent
            {
                TenantId = context.TenantId,
                UserGuid = context.UserGuid,
                SessionGuid = context.SessionGuid,
                EventName = request.EventName,
                Route = request.Route,
                Status = request.Status,
                Metadata = request.Metadata
            };

            _db.Logs.Add(log);
            await _db.SaveChangesAsync();

            return log;
        }

        public async Task<List<LogEvent>> GetLogsAsync(LogQueryParams query, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

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
              .Where(log => log.TenantId == context.TenantId)
              .OrderByDescending(logsQuery => logsQuery.CreatedAt)
              .Skip(offset)
              .Take(limit)
              .ToListAsync();
        }

        public async Task<List<LogEvent>> GetLogsByIdAsync(Guid guid, LogQueryParams query, LogGuidType guidType, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

            var logsQuery = _db.Logs.Where(log => log.TenantId == context.TenantId);

            var end = query.End ?? DateTime.UtcNow;
            var start = query.Start ?? end.AddHours(-12);

            logsQuery = logsQuery
              .Where(log => log.CreatedAt >= start && log.CreatedAt <= end);

            switch (guidType)
            {
                case LogGuidType.UserGuid:
                    logsQuery = logsQuery.Where(log => log.UserGuid == guid);
                    break;
                case LogGuidType.SessionGuid:
                    logsQuery = logsQuery.Where(log => log.SessionGuid == guid);
                    break;
                default:
                    throw new ArgumentException("Invalid guidType");
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
              .OrderByDescending(log => log.CreatedAt)
              .Skip(offset)
              .Take(limit)
              .ToListAsync();
        }
    }
}