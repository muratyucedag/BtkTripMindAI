namespace BtkTripMindAI.Models.CalendarModels
{
    public class GoogleCalendarSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = "BtkTripMindAI";
        public string CalendarId { get; set; } = "primary";
        public string UserEmail { get; set; } = string.Empty;
    }
}
