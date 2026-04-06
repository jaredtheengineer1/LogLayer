using System.Linq;
using LogLayer.Data;
using LogLayer.Dtos;
using LogLayer.Models;
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

        public async Task<List<LogEvent>> GetLogsAsync(LogQueryParams query)
        {
            return await _db.Logs
              .Where(e => e.CreatedAt >= (query.Start ?? DateTime.UtcNow.AddHours(-12)) &&
                          e.CreatedAt <= (query.End ?? DateTime.UtcNow) &&
                          (string.IsNullOrWhiteSpace(query.EventName) || e.EventName == query.EventName))
              .ToListAsync();
        }
    }
}