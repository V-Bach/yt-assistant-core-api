using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeLearningAssistant.Api.Models
{
    [Table("videos")]
    public class Video
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string VideoId { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Transcript { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
