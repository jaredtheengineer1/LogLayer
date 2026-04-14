using System.Text.Json;
namespace LogLayer.Dtos
{
    public class ExportedLogDto
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = default!;
        public string? Route { get; set; }
        public string? Status { get; set; }
        public JsonDocument? Metadata { get; set; }
        public DateTime Timestamp { get; set; }
    }
}