namespace BtkTripMindAI.Models.CalendarModels
{
    public class CalendarEventExtractionResult
    {
        public bool Found { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string TimeZone { get; set; } = "Europe/Istanbul";
    }
}
