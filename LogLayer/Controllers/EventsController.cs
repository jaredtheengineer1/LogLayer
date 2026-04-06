using LogLayer.Dtos;
using LogLayer.Services;
using LogLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

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
}