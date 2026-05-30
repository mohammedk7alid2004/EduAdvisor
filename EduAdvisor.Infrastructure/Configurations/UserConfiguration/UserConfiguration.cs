using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.AuthModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations.UserConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FullName)
                .HasMaxLength(50)
                .IsRequired();


        }
    }
}
