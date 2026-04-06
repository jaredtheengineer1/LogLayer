using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
namespace LogLayer.Models
{
    public class LogEvent
    {
        [Column("id")]
        public int Id { get; set; } // primary key
        [Column("log_id")]
        public Guid LogId { get; set; } = Guid.NewGuid();
        [Column("tenant_id")]
        public Guid TenantId { get; set; }
        [Column("user_guid")]
        public Guid UserGuid { get; set; }
        [Column("session_guid")]
        public Guid SessionGuid { get; set; }
        [Column("event")]
        public string EventName { get; set; } = default!;
        [Column("route")]
        public string? Route { get; set; }
        [Column("status")]
        public string? Status { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("metadata")]
        public JsonDocument? Metadata { get; set; }
    }
}