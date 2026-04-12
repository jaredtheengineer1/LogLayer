using LogLayer.Data;
using LogLayer.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LogLayer.Services
{
    public class EventService
    {
        private readonly ApplicationDbContext _db;

        public EventService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<EventCountDto>> GetEventCountsAsync(LogQueryParams query, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

            var end = query.End ?? DateTime.UtcNow;
            var start = query.Start ?? end.AddHours(-12);

            var logsQuery = _db.Logs
                .Where(log => log.TenantId == context.TenantId)
                .Where(log => log.CreatedAt >= start && log.CreatedAt <= end);

            return await logsQuery
              .GroupBy(log => log.EventName)
              .Select(ec => new EventCountDto
              {
                  EventName = ec.Key,
                  Count = ec.Count()
              })
              .OrderByDescending(ev => ev.Count)
              .ToListAsync();
        }

        public async Task<List<EventTimeBucketDto>> GetEventTimelineAsync(string eventName, LogQueryParams query, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

            var end = query.End ?? DateTime.UtcNow;
            var start = query.Start ?? end.AddHours(-12);

            var logs = await _db.Logs
        .Where(log => log.TenantId == context.TenantId)
        .Where(log => log.EventName == eventName)
        .Where(log => log.CreatedAt >= start && log.CreatedAt <= end)
        .ToListAsync();

            // Group in memory (fine for now)
            return logs
                .GroupBy(log => new DateTime(
                    log.CreatedAt.Year,
                    log.CreatedAt.Month,
                    log.CreatedAt.Day,
                    log.CreatedAt.Hour,
                    0,
                    0))
                .Select(g => new EventTimeBucketDto
                {
                    Time = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Time)
                .ToList();
        }
    }
}