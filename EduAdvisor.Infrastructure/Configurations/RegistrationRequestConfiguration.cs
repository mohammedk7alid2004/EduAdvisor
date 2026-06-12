using EduAdvisor.Domain.Entities.AcademicModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public sealed class RegistrationRequestConfiguration
    : IEntityTypeConfiguration<RegistrationRequest>
{
    public void Configure(EntityTypeBuilder<RegistrationRequest> builder)
    {
        builder.ToTable("RegistrationRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.SubmittedAt)
            .IsRequired();

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Semester)
            .WithMany()
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Enrollments)
            .WithOne(x => x.RegistrationRequest)
            .HasForeignKey(x => x.RegistrationRequestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.Enrollments)
            .HasField("_enrollments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}