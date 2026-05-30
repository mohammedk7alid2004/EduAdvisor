using EduAdvisor.Domain.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public class SubjectPrerequisiteConfiguration : IEntityTypeConfiguration<SubjectPrerequisite>
{
    public void Configure(EntityTypeBuilder<SubjectPrerequisite> builder)
    {
        builder.ToTable("SubjectPrerequisites");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new { x.SubjectId, x.PrerequisiteSubjectId })
            .IsUnique();
    }
}