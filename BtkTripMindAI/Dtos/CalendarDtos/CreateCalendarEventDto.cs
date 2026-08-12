namespace BtkTripMindAI.Dtos.CalendarDtos
{
    public class CreateCalendarEventDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string TimeZone { get; set; } = "Europe/Istanbul";
    }
}
