using EduAdvisor.Domain.Entities.Universities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Email)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.Website)
            .HasMaxLength(300)
            .IsRequired(false);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Faculties)
            .WithOne(x => x.University)
            .HasForeignKey(x => x.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}