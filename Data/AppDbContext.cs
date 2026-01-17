using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YoutubeLearningAssistant.Api.Models; // Để nó tìm thấy class Video của bạn

namespace YoutubeLearningAssistant.Api.Data;

// PHẢI CÓ ": DbContext" để kế thừa tính năng của thư viện
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Khai báo bảng Videos sẽ xuất hiện trong MySQL
    public DbSet<Video> Videos { get; set; }
}