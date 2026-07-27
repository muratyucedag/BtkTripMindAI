using BtkTripMindAI.Dtos.BudgetDtos;

namespace BtkTripMindAI.Services.BudgetServices
{
    public interface IBudgetService
    {
        Task<BudgetResult> CalculateBudgetAsync(string city, string? country, int dayCount, int adultCount, int childCount);
    }
}
