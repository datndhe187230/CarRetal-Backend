using CarRental_BE.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRental_BE.Chatbot.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatBotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Câu hỏi không hợp lệ.");

            try
            {
                var answer = await _chatbotService.GetChatbotResponse(request.Question);
                return Ok(new { answer, from = "chatbot" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class ChatRequest
    {
        public string Question { get; set; } = "";
    }

}
