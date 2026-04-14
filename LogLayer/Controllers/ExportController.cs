using System.Text;
using LogLayer.Dtos;
using LogLayer.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly ExportService _exportService;
    private readonly RequestContext _context;

    public ExportController(ExportService service, RequestContext context)
    {
        _exportService = service;
        _context = context;
    }

    [HttpGet("csv")]
    public async Task<IActionResult> ExportLogsToCsv([FromQuery] LogQueryParams query)
    {
        var csvData = await _exportService.ExportLogsToCsvAsync(query, _context);
        var fileName = $"logs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        return File(Encoding.UTF8.GetBytes(csvData), "text/csv", fileName);
    }

    [HttpGet("json")]
    public async Task<IActionResult> ExportLogsToJson([FromQuery] LogQueryParams query)
    {
        var jsonData = await _exportService.ExportLogsToJsonAsync(query, _context);
        var fileName = $"logs_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
        return File(new System.Text.UTF8Encoding().GetBytes(jsonData), "application/json", fileName);
    }

    [HttpGet("xml")]
    public async Task<IActionResult> ExportLogsToXml([FromQuery] LogQueryParams query)
    {
        var xmlData = await _exportService.ExportLogsToXmlAsync(query, _context);
        var fileName = $"logs_{DateTime.UtcNow:yyyyMMddHHmmss}.xml";
        return File(Encoding.UTF8.GetBytes(xmlData), "application/xml", fileName);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> ExportLogsToPdf([FromQuery] LogQueryParams query)
    {
        try
        {
            var pdfData = await _exportService.ExportLogsToPdfAsync(query, _context);
            var fileName = $"logs_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            return File(pdfData, "application/pdf", fileName);
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, "PDF export is not implemented yet.");
        }
    }
}