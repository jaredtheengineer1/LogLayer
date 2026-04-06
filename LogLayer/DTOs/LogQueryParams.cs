namespace LogLayer.Dtos
{
    public class LogQueryParams
    {
        public Guid? UserGuid { get; set; }
        public Guid? SessionGuid { get; set; }
        public string? EventName { get; set; }
        public string? Route { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int Limit { get; set; } = 100;
        public int Offset { get; set; } = 0;
    }
}