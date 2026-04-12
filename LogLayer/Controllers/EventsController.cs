using LogLayer.Dtos;
using LogLayer.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly RequestContext _context;

    public EventsController(EventService eventService, RequestContext context)
    {
        _eventService = eventService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] LogQueryParams query)
    {
        var result = await _eventService.GetEventCountsAsync(query, _context);
        return Ok(result);
    }

    [HttpGet("{eventName}")]
    public async Task<IActionResult> GetEventTimeline(string eventName, [FromQuery] LogQueryParams query)
    {
        var result = await _eventService.GetEventTimelineAsync(eventName, query, _context);
        return Ok(result);
    }
}