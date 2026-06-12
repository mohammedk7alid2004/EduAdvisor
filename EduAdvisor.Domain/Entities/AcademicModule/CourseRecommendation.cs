using EduAdvisor.Domain.Entities.Base;
using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Domain.Entities.Semesters;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Domain.Entities.AcademicModule;

public  class CourseRecommendation : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid SemesterId { get; private set; }

    public CourseDifficulty Difficulty { get; private set; } // Easy, Medium, Hard
    public string Description { get; private set; } = string.Empty; // AI written detailed text about the course
    public string Reasoning { get; private set; } = string.Empty; // Why AI picked this for the specific student
    public decimal ExpectedGpaImpact { get; private set; } // Predicted improvement on cumulative GPA

    public Student Student { get; private set; } = default!;
    public Course Course { get; private set; } = default!;
    public Semester Semester { get; private set; } = default!;

    private CourseRecommendation() { }

    public CourseRecommendation(
        Guid studentId,
        Guid courseId,
        Guid semesterId,
        CourseDifficulty difficulty,
        string description,
        string reasoning,
        decimal expectedGpaImpact)
    {
        StudentId = studentId;
        CourseId = courseId;
        SemesterId = semesterId;
        Difficulty = difficulty;
        Description = description;
        Reasoning = reasoning;
        ExpectedGpaImpact = expectedGpaImpact;
    }
}