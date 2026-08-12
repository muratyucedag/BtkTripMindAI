using BtkTripMindAI.Models.CalendarModels;

namespace BtkTripMindAI.Agent.Extractors
{
    public interface ICalendarEventExtractor
    {
        Task<CalendarEventExtractionResult> ExtractAsync(string message);
    }
}
