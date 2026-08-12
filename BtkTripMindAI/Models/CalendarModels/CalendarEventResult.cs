namespace BtkTripMindAI.Models.CalendarModels
{
    public class CalendarEventResult
    {
        public bool IsSuccess { get; set; }
        public string? EventId { get; set; }
        public string? EventLink { get; set; }
        public string? Title { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Message { get; set; }
    }
}
