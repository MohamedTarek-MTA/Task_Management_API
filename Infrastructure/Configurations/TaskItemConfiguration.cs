using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Infrastructure.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder) 
        {
            // TaskItem & Project: One-to-Many relationship
            builder.HasKey(t => t.Id);
            builder.HasOne(t => t.Project)
                .WithMany(p => p.TaskItems)
                .HasForeignKey(ti => ti.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskItem & User: One-to-Many relationship 

            builder.HasOne(ti => ti.AssignedUser)
                .WithMany(u => u.TaskItems)
                .HasForeignKey(ti => ti.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // TaskItem & TaskHistory: One-to-Many relationship

            builder.HasMany(th=> th.TaskHistories)
                .WithOne()
                .HasForeignKey(ti => ti.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(ti => ti.CreatedAt);
        }
    }
}
