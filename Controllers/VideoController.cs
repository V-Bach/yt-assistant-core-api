using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json; 
using YoutubeLearningAssistant.Api.Data;
using YoutubeLearningAssistant.Api.Models;
using System.Text.Json.Serialization;

namespace YoutubeLearningAssistant.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VideoController(AppDbContext context)
        {
            _context = context;
        }

        // ĐỔI TÊN THÀNH "save" ĐỂ HẾT LỖI 404
        [HttpPost("save")]
        public async Task<IActionResult> SaveVideo([FromBody] VideoSaveRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Summary))
                return BadRequest("Dữ liệu không hợp lệ.");

            try
            {
                // Chỉ việc lưu dữ liệu mà Extension gửi sang
                var videoEntity = new Video
                {
                    VideoId = request.VideoId,
                    Title = request.Title,
                    Summary = request.Summary, // Đây là summary AI từ Python gửi qua Extension
                    Transcript = "", // Có thể để trống nếu Extension không gửi
                    CreatedAt = DateTime.Now
                };

                _context.Videos.Add(videoEntity);
                await _context.SaveChangesAsync();

                return Ok(new { message = "C# đã lưu thành công vào MySQL!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi lưu Database: {ex.Message}");
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

    // Class hứng dữ liệu phải khớp với JSON từ popup.js gửi sang
    public class VideoSaveRequest
    {
        [JsonPropertyName("videoId")]
        public string VideoId { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("summary")]
        public string Summary { get; set; }
    }
}