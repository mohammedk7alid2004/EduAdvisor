using EduAdvisor.Domain.Entities.Semesters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.ToTable("Semesters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Year)
            .IsRequired();

        builder.HasIndex(x => new { x.Name, x.Year })
            .IsUnique();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.IsRegistrationOpen)
            .IsRequired();

        builder.HasMany(x => x.Enrollments)
            .WithOne(x => x.Semester)
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}