using Microsoft.EntityFrameworkCore;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Infrastructure.Date
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<TaskItem> taskItems { get; set; }
        public DbSet<TaskHistory> taskHistories { get; set; }
    }
}
