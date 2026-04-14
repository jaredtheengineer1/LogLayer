using System.Text.Json;
using LogLayer.Data;
using LogLayer.Dtos;
using LogLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace LogLayer.Services
{
    public class ExportService
    {
        private readonly ApplicationDbContext _dbContext;

        public ExportService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> ExportLogsToCsvAsync(LogQueryParams query, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

            var logs = await BuildBaseQuery(query, context).ToListAsync();


            var csvLines = new List<string>
            {
                "LogId,TenantId,EventName,Route,Status,UserGuid,SessionGuid,CreatedAt"
            };

            csvLines.AddRange(logs.Select(log =>
                $"{EscapeForCsv(log.LogId.ToString())},{EscapeForCsv(log.TenantId.ToString())},{EscapeForCsv(log.EventName)},{EscapeForCsv(log.Route ?? "")},{EscapeForCsv(log.Status ?? "")},{EscapeForCsv(log.UserGuid.ToString())},{EscapeForCsv(log.SessionGuid.ToString())},{log.CreatedAt:o}"));

            var csvContent = string.Join(Environment.NewLine, csvLines);
            return csvContent;
        }

        public async Task<string> ExportLogsToJsonAsync(LogQueryParams query, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

            var logs = await BuildBaseQuery(query, context)
            .Select(
                log => new ExportedLogDto
                {
                    Id = log.LogId,
                    EventName = log.EventName,
                    Route = log.Route,
                    Status = log.Status,
                    Metadata = log.Metadata,
                    Timestamp = log.CreatedAt,
                }
            ).ToListAsync();

            return JsonSerializer.Serialize(logs, JsonOptions);
        }


        public async Task<string> ExportLogsToXmlAsync(LogQueryParams query, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

            var logs = await BuildBaseQuery(query, context).ToListAsync();

            var xmlDoc = new System.Xml.XmlDocument();
            var root = xmlDoc.CreateElement("Logs");
            xmlDoc.AppendChild(root);

            foreach (var log in logs)
            {
                var logElement = xmlDoc.CreateElement("Log");

                void AppendChild(string name, string value)
                {
                    var child = xmlDoc.CreateElement(name);
                    child.InnerText = value;
                    logElement.AppendChild(child);
                }

                AppendChild("LogId", log.LogId.ToString());
                AppendChild("TenantId", log.TenantId.ToString());
                AppendChild("EventName", log.EventName);
                AppendChild("Route", log.Route ?? "");
                AppendChild("Status", !string.IsNullOrEmpty(log.Status) ? log.Status.ToString() : "");
                AppendChild("UserGuid", log.UserGuid.ToString());
                AppendChild("SessionGuid", log.SessionGuid.ToString());
                AppendChild("CreatedAt", log.CreatedAt.ToString("o"));

                root.AppendChild(logElement);
            }

            using var stringWriter = new StringWriter();
            using var xmlTextWriter = System.Xml.XmlWriter.Create(stringWriter, new System.Xml.XmlWriterSettings { Indent = true });
            xmlDoc.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            return stringWriter.GetStringBuilder().ToString();
        }

        public async Task<string> ExportLogsToPdfAsync(LogQueryParams query, RequestContext context)
        {
            // PDF export is more complex and typically requires a library like QuestPDF.
            throw new NotImplementedException("PDF export is not implemented yet.");
        }

        private IQueryable<LogEvent> BuildBaseQuery(LogQueryParams query, RequestContext context)
        {
            if (context.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is not set in RequestContext");
            }

            var end = query.End ?? DateTime.UtcNow;
            var start = query.Start ?? end.AddHours(-12);
            var limit = Math.Min(query.Limit, 5000);
            var offset = Math.Max(query.Offset, 0);
            var logs = _dbContext.Logs
                .Where(log => log.TenantId == context.TenantId)
                .Where(log => log.CreatedAt >= start && log.CreatedAt <= end)
                .OrderByDescending(log => log.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .AsNoTracking();

            if (query.UserGuid.HasValue)
            {
                logs = logs.Where(log => log.UserGuid == query.UserGuid);
            }

            if (query.SessionGuid.HasValue)
            {
                logs = logs.Where(log => log.SessionGuid == query.SessionGuid);
            }

            if (!string.IsNullOrWhiteSpace(query.EventName))
            {
                logs = logs.Where(log => log.EventName == query.EventName);
            }

            if (!string.IsNullOrWhiteSpace(query.Route))
            {
                logs = logs.Where(log => log.Route == query.Route);
            }

            return logs;
        }
        private string EscapeForCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}