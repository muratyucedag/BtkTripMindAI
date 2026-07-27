namespace BtkTripMindAI.Models.TravelModels
{
    public class TravelRequestDetails
    {
        public string? City { get; set; }
        public string? Country { get; set; }
        public int DayCount { get; set; } = 3;
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public string BudgetLevel { get; set; } = "Medium";
    }
}