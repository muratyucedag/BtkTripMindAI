namespace BtkTripMindAI.Dtos.BudgetDtos
{
    public class BudgetResult
    {
        public bool IsSuccess { get; set; }
        public string Currency { get; set; } = "EUR";
        public decimal TotalAmount { get; set; }
        public List<BudgetItem> Items { get; set; } = new();
        public string Description { get; set; }
    }
}
