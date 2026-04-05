using LogLayer.Dtos;
using LogLayer.Services;
using Microsoft.AspNetCore.Mvc;

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
    if (string.IsNullOrWhiteSpace(request.Event))
    {
      return BadRequest("EventName is required.");
    }

    var log = await _logService.CreateLogAsync(request, _context);

    return Ok(log);
  }
}