using CarRental_BE.Models.NewEntities;
using CarRental_BE.Repositories;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CarRental_BE.Chatbot
{
    public class ChatbotServiceImpl : IChatbotService
    {
        private readonly string _apiKey;
        private readonly ICarRepository _carRepository;

        private static readonly Dictionary<string, string> PredefinedAnswers = new(StringComparer.OrdinalIgnoreCase)
        {
            { "giờ làm việc", "Chúng tôi làm việc từ 8h sáng đến 5h chiều, từ thứ 2 đến thứ 6." },
            { "địa chỉ", "Chúng tôi ở số 123 Đường ABC, Quận 1, TP.HCM." }
        };

        private static readonly List<string> CarQueryKeywords = new()
        {
            "xe nao", "gia thue", "thue xe", "xe co san",
            "available cars", "car available", "car rental", "rental price", "how much", "what cars"
        };

        public ChatbotServiceImpl(IConfiguration configuration, ICarRepository carRepository)
        {
            _apiKey = configuration["GeminiApiKey"];
            _carRepository = carRepository;
        }

        public async Task<string> GetChatbotResponse(string userMessage)
        {
            // 1. Trả lời nhanh nếu có trong predefined
            foreach (var (key, value) in PredefinedAnswers)
            {
                if (userMessage.Contains(key, StringComparison.OrdinalIgnoreCase))
                    return value;
            }

            // 2. Kiểm tra có liên quan đến xe
            string normalizedMessage = RemoveDiacritics(userMessage).ToLower();
            bool isCarQuery = CarQueryKeywords.Any(keyword => normalizedMessage.Contains(keyword));

            string contextText = "";
            if (isCarQuery)
            {
                var cars = await _carRepository.GetAllWithFeedback();
                if (!cars.Any())
                    contextText = "Hiện tại không có xe nào trong danh sách.";
                else
                {
                    var sb = new StringBuilder("Danh sách xe hiện có:\n");
                    foreach (var car in cars)
                    {
                        sb.AppendLine($"- {car.Brand} {car.Model}, màu {car.Color}, giá thuê {car.BasePrice:N0}đ/ngày, đánh giá trung bình: {car.AverageRating:F1}⭐️");
                    }

                    contextText = sb.ToString();
                }
            }

            // 3. Gửi Gemini
            return await QueryGeminiAsync(contextText, userMessage);
        }

        private async Task<string> QueryGeminiAsync(string context, string userMessage)
        {
            var prompt = $"{context}\n\nCâu hỏi: {userMessage}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={_apiKey}";

            try
            {
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return $"❌ Gọi API Gemini thất bại: {responseString}";

                var jsonDoc = JsonDocument.Parse(responseString);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out var contentObj) &&
                    contentObj.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var textValue))
                {
                    return textValue.GetString() ?? "Không có phản hồi từ chatbot.";
                }

                return "Không thể phân tích phản hồi từ Gemini.";
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi khi gọi Gemini: {ex.Message}";
            }
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
