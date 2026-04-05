using LogLayer.Data;
using LogLayer.Dtos;
using LogLayer.Models;

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
        TenantId = Guid.Empty,
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
  }
}