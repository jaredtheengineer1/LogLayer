using System.Text.Json;
namespace LogLayer.Dtos
{
    public class CreateLogRequest
    {
        public string EventName { get; set; } = default!;
        public string? Route { get; set; }
        public string? Status { get; set; }
        public JsonDocument? Metadata { get; set; }
    }
}