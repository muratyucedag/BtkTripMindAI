namespace BtkTripMindAI.Services.ImageServices
{
    public interface IGooglePlacePhotoService
    {
        Task<string?> GetPhotoUrlAsync(string? photoName, int maxWidth = 1200);
    }
}
