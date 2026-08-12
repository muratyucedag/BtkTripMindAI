namespace BtkTripMindAI.Models.EmailModels
{
    public interface IEmailExtractor
    {
        Task<EmailExtractionResult> ExtractAsync(string message);
    }
}