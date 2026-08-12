using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Dtos.BudgetDtos;
using BtkTripMindAI.Models.FlightModels;
using BtkTripMindAI.Models.HotelModels;
using BtkTripMindAI.Models.RestaurantModels;
using BtkTripMindAI.Models.RouteModels;
using BtkTripMindAI.Models.WeatherModels;

namespace BtkTripMindAI.Agent.Responses
{
    public class AgentResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public TravelIntent Intent { get; set; }
        public string? ToolName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public WeatherResult? Weather { get; set; }
        public string? CoverImageUrl { get; set; }
        public RouteResult? Route { get; set; }
        public RestaurantResult? Restaurant { get; set; }
        public HotelResult? Hotel { get; set; }
        public BudgetResult? Budget { get; set; }
        public int DayCount { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string? BudgetLevel { get; set; }
        public string? EmailAddress { get; set; }
        public bool EmailSent { get; set; }
        public FlightResult? Flight { get; set; }
    }
}