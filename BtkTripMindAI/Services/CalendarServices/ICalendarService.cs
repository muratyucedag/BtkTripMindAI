using BtkTripMindAI.Dtos.CalendarDtos;
using BtkTripMindAI.Models.CalendarModels;

namespace BtkTripMindAI.Services.CalendarServices
{
    public interface ICalendarService
    {
        Task<CalendarEventResult> CreateEventAsync(CreateCalendarEventDto request);
    }
}
