using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json; // Thư viện quan trọng để gửi JSON
using YoutubeLearningAssistant.Api.Data;
using YoutubeLearningAssistant.Api.Models;

namespace YoutubeLearningAssistant.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        // Inject thêm IHttpClientFactory để quản lý việc gọi API ra ngoài
        public VideoController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeVideo([FromBody] VideoAnalysisRequest request)
        {
            if (string.IsNullOrEmpty(request.Transcript))
                return BadRequest("Transcript cannot be empty.");

            try
            {
                // --- BƯỚC 1: GỬI DỮ LIỆU SANG PYTHON AI SERVICE ---
                var pythonServiceUrl = "http://127.0.0.1:8000/ai/process";
                var client = _httpClientFactory.CreateClient();

                // Gửi request sang Python
                var response = await client.PostAsJsonAsync(pythonServiceUrl, request);

                string aiSummary = "";

                if (response.IsSuccessStatusCode)
                {
                    // Đọc kết quả từ Python (phần ai_analysis mà bạn viết ở main.py)
                    var aiResponse = await response.Content.ReadFromJsonAsync<PythonAIResponse>();
                    aiSummary = aiResponse?.ai_analysis ?? "Không có phản hồi từ AI.";
                }
                else
                {
                    aiSummary = "Lỗi: Không thể kết nối với AI Service.";
                }

                // --- BƯỚC 2: LƯU VÀO DATABASE MYSQL ---
                var videoEntity = new Video
                {
                    VideoId = request.VideoId,
                    Title = request.Title,
                    Transcript = request.Transcript,
                    Summary = aiSummary, // Lưu bản tóm tắt THẬT từ Gemini
                    CreatedAt = DateTime.Now
                };

                _context.Videos.Add(videoEntity);
                await _context.SaveChangesAsync();

                // --- BƯỚC 3: TRẢ KẾT QUẢ VỀ CHO EXTENSION ---
                return Ok(new
                {
                    Message = "AI đã xử lý và lưu thành công!",
                    VideoDbId = videoEntity.Id,
                    Summary = aiSummary
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var history = await _context.Videos
                    .OrderByDescending(v => v.CreatedAt)
                    .ToListAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllHistory()
        {
            try
            {
                var allVideos = await _context.Videos.ToListAsync();
                _context.Videos.RemoveRange(allVideos);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đã xóa sạch kho kiến thức!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    }

    // Class phụ để hứng dữ liệu từ Python trả về
    public class PythonAIResponse
    {
        public string status { get; set; }
        public string ai_analysis { get; set; }
    }
}