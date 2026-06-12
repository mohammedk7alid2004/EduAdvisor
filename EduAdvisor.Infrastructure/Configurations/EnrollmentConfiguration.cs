using EduAdvisor.Domain.Entities.Enrollments;
using EduAdvisor.Domain.Enums.University;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
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

        builder.Property(x => x.Grade)
            .HasColumnType("decimal(4,2)")
            .IsRequired(false);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.ReviewedAt)
            .IsRequired(false);

        builder.Property(x => x.GradedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Semester)
            .WithMany()
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ReviewedByAdvisor)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByAdvisorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}