using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json; 
using YoutubeLearningAssistant.Api.Data;
using YoutubeLearningAssistant.Api.Models;
using System.Text.Json.Serialization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

        [HttpPost("save")]
        public async Task<IActionResult> SaveVideo([FromBody] VideoSaveRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Summary))
                return BadRequest("Dữ liệu không hợp lệ.");

            try
            {
                var videoEntity = new Video
                {
                    VideoId = request.VideoId,
                    Title = request.Title,
                    Summary = request.Summary, 
                    Transcript = "",
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

        [HttpGet("export-pdf/{id}")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var video = await _context.Videos.FindAsync(id);

            if (video == null)
                return NotFound(new { message = "Không tìm thấy video này bro ơi!" });

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("BẢN TÓM TẮT KIẾN THỨC").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{video.Title}").FontSize(14).Italic();
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().Text($"Ngày học: {video.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(15).Text("NỘI DUNG CHI TIẾT").Bold().FontSize(14);

                        col.Item().PaddingTop(10).Text(video.Summary);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"{video.VideoId}_Summary.pdf");
        }
    }

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