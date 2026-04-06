using LogLayer.Dtos;
using LogLayer.Services;
using Microsoft.AspNetCore.Mvc;
using LogLayer.Models;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
    private readonly LogService _logService;
    private readonly RequestContext _context;

    public LogsController(LogService logService, RequestContext context)
    {
        _logService = logService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLog([FromBody] CreateLogRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EventName))
        {
            return BadRequest("EventName is required.");
        }

        var log = await _logService.CreateLogAsync(request, _context);

        return Ok(log);
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] LogQueryParams query)
    {
        var logs = await _logService.GetLogsAsync(query);
        return Ok(logs);
    }

    [HttpGet("/user/{userId:guid}")]
    public async Task<IActionResult> GetLogsByUserGuid(Guid userId, [FromQuery] LogQueryParams query)
    {
        var logs = await _logService.GetLogsByIdAsync(userId, query, LogGuidType.UserGuid);
        return Ok(logs);
    }

    [HttpGet("/tenant/{tenantId:guid}")]
    public async Task<IActionResult> GetLogsByTenantId(Guid tenantId, [FromQuery] LogQueryParams query)
    {
        var logs = await _logService.GetLogsByIdAsync(tenantId, query, LogGuidType.TenantId);
        return Ok(logs);
    }

    [HttpGet("/session/{sessionId:guid}")]
    public async Task<IActionResult> GetLogsBySessionId(Guid sessionId, [FromQuery] LogQueryParams query)
    {
        var logs = await _logService.GetLogsByIdAsync(sessionId, query, LogGuidType.SessionGuid);
        return Ok(logs);
    }
}