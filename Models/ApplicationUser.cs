using Microsoft.AspNetCore.Identity;

namespace YoutubeLearningAssistant.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
