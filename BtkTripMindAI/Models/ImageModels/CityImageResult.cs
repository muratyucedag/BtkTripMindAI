namespace BtkTripMindAI.Models.ImageModels
{
    public class CityImageResult
    {
        public bool IsSuccess { get; set; }
        public string? ImageUrl { get; set; }
        public string? Photographer { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
