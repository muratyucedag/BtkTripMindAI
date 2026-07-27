using BtkTripMindAI.Dtos.BudgetDtos;

namespace BtkTripMindAI.Services.BudgetServices
{
    public class BudgetService : IBudgetService
    {
        public Task<BudgetResult> CalculateBudgetAsync(string city, string? country, int dayCount, int adultCount, int childCount)
        {
            if (dayCount <= 0)
            {
                return Task.FromResult(new BudgetResult
                {
                    IsSuccess = false,
                    Currency = "EUR",
                    Description = "Gün sayısı 1 veya daha büyük olmalıdır."
                });
            }

            if (adultCount <= 0)
            {
                return Task.FromResult(new BudgetResult
                {
                    IsSuccess = false,
                    Currency = "EUR",
                    Description = "En az 1 yetişkin bilgisi girilmelidir."
                });
            }

            if (childCount < 0)
            {
                childCount = 0;
            }

            int totalPeople = adultCount + childCount;

            /*
             * Bu değerler şimdilik örnek ve kural tabanlıdır.
             * İleride OpenAI veya gerçek fiyat servislerinden alınacaktır.
             */

            decimal nightlyHotelPrice = 180m;

            decimal dailyFoodPricePerAdult = 45m;
            decimal dailyFoodPricePerChild = 25m;

            decimal dailyTransportationPricePerPerson = 12m;

            decimal dailyActivityPricePerAdult = 30m;
            decimal dailyActivityPricePerChild = 15m;

            decimal accommodationAmount = nightlyHotelPrice * dayCount;
            decimal foodAmount = (adultCount * dailyFoodPricePerAdult + childCount * dailyFoodPricePerChild) * dayCount;
            decimal transportationAmount = totalPeople * dailyTransportationPricePerPerson * dayCount;
            decimal activityAmount = (adultCount * dailyActivityPricePerAdult + childCount * dailyActivityPricePerChild) * dayCount;
            decimal totalAmount = accommodationAmount + foodAmount + transportationAmount + activityAmount;

            var result = new BudgetResult
            {
                IsSuccess = true,
                Currency = "EUR",
                TotalAmount = totalAmount,
                Description = $"{city} için {adultCount} yetişkin, " + $"{childCount} çocuk ve {dayCount} günlük " + "orta segment bir seyahat dikkate alınarak " + "tahmini bütçe oluşturulmuştur.",

                Items = new List<BudgetItem>
                {
                    new BudgetItem
                    {
                        Category = "Accommodation",
                        Amount = accommodationAmount,
                        Percentage = CalculatePercentage(accommodationAmount,  totalAmount)
                    },

                    new BudgetItem
                    {
                        Category = "Food",
                        Amount = foodAmount,
                        Percentage = CalculatePercentage(foodAmount,totalAmount)
                    },

                    new BudgetItem
                    {
                        Category = "Transportation",
                        Amount = transportationAmount,
                        Percentage = CalculatePercentage(transportationAmount, totalAmount)
                    },

                    new BudgetItem
                    {
                        Category = "Activities",
                        Amount = activityAmount,
                        Percentage = CalculatePercentage(activityAmount, totalAmount)
                    }
                }
            };

            return Task.FromResult(result);
        }
        private static int CalculatePercentage(decimal amount, decimal totalAmount)
        {
            if (totalAmount <= 0)
            {
                return 0;
            }

            return (int)Math.Round(amount / totalAmount * 100, MidpointRounding.AwayFromZero);
        }
    }
}