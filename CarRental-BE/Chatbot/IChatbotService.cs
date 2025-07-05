namespace CarRental_BE.Chatbot
{
    public interface IChatbotService
    {
        Task<string> GetChatbotResponse(string userMessage);
    }
}
