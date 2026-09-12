using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Infrastructure.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            
            builder.Property(u => u.Role)
                .HasConversion<string>();

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();
        }
    }
}
