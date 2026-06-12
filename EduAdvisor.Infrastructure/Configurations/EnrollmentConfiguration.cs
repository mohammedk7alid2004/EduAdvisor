using EduAdvisor.Domain.Entities.Enrollments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public sealed class EnrollmentConfiguration
    : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CoursePercentage)
            .HasColumnType("decimal(5,2)")
            .IsRequired(false);

        builder.Property(x => x.CourseGpa)
            .HasColumnType("decimal(3,2)")
            .IsRequired(false);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.SemesterCourse)
            .WithMany()
            .HasForeignKey(x => x.SemesterCourseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.RegistrationRequest)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.RegistrationRequestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ReviewedByAdvisor)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByAdvisorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}