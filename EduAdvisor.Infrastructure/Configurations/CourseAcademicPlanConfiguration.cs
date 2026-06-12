using EduAdvisor.Domain.Entities.AcademicModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduAdvisor.Infrastructure.Configurations;

public class CourseAcademicPlanConfiguration : IEntityTypeConfiguration<CourseAcademicPlan>
{
    public void Configure(EntityTypeBuilder<CourseAcademicPlan> builder)
    {
        builder.ToTable("CourseAcademicPlans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Level)
            .IsRequired();

        builder.Property(x => x.StandardSemester)
            .IsRequired();

        builder.HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class SemesterCourseConfiguration : IEntityTypeConfiguration<SemesterCourse>
{
    public void Configure(EntityTypeBuilder<SemesterCourse> builder)
    {
        builder.ToTable("SemesterCourses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(x => x.Semester)
            .WithMany()
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CourseAcademicPlan)
            .WithMany()
            .HasForeignKey(x => x.CourseAcademicPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}