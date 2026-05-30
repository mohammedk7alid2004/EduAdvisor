using EduAdvisor.Domain.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public class SubjectOfferingConfiguration : IEntityTypeConfiguration<SubjectOffering>
{
    public void Configure(EntityTypeBuilder<SubjectOffering> builder)
    {
        builder.ToTable("SubjectOfferings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Level)
            .IsRequired();

        builder.Property(x => x.MaxCapacity)
            .IsRequired();

        builder.Property(x => x.CurrentEnrollment)
            .IsRequired();

        builder.HasIndex(x => new { x.SubjectId, x.SemesterId, x.DepartmentId })
            .IsUnique();

        builder.HasOne(x => x.Subject)
            .WithMany()
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Semester)
            .WithMany()
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}