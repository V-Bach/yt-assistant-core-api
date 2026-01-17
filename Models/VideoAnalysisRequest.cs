namespace YoutubeLearningAssistant.Api.Models
{
    public class VideoAnalysisRequest
    {
        public string VideoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Transcript { get; set; } = string.Empty;
    }
}
