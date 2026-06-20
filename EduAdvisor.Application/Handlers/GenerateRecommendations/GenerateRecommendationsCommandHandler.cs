using EduAdvisor.Application.Commands.GenerateRecommendations;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.AiRecommendation;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.ExternalServices;
using EduAdvisor.Domain.Entities.AcademicModule;
using EduAdvisor.Domain.Enums.University;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.GenerateRecommendations;

public sealed class GenerateRecommendationsCommandHandler(
    IApplicationDbContext context,
    IAiRecommendationService aiRecommendationService)
    : IRequestHandler<
        GenerateRecommendationsCommand,
        Result<List<CourseRecommendation>>>
{
    public async Task<Result<List<CourseRecommendation>>> Handle(
        GenerateRecommendationsCommand request,
        CancellationToken cancellationToken)
    {
        var studentExists = await context.Students
            .AsNoTracking()
            .AnyAsync(
                student => student.Id == request.StudentId,
                cancellationToken);

        if (!studentExists)
        {
            return Result<List<CourseRecommendation>>
                .NotFound("Student not found.");
        }

        var semesterExists = await context.Semesters
            .AsNoTracking()
            .AnyAsync(
                semester => semester.Id == request.SemesterId,
                cancellationToken);

        if (!semesterExists)
        {
            return Result<List<CourseRecommendation>>
                .NotFound("Semester not found.");
        }

        var aiRequest = new AiRecommendationRequestDto
        {
            StudentMajor = request.StudentMajor,
            CurrentGpa = request.CurrentGpa,
            Level = request.Level,
            CompletedHours = request.CompletedHours,
            RegisteredHours = request.RegisteredHours,
            Semester = request.Semester,
            IsGraduationSemester = request.IsGraduationSemester,
            AvailableCourses = request.AvailableCourses
                .Select(course => new AiAvailableCourseDto
                {
                    CourseCode = course.CourseCode,
                    PrereqGrades = course.PrereqGrades
                })
                .ToList()
        };

        var aiResponse =
            await aiRecommendationService.GetRecommendationsAsync(
                aiRequest,
                cancellationToken);

        if (aiResponse?.Recommendations is not { Count: > 0 })
        {
            return Result<List<CourseRecommendation>>.Error(
                "Unable to generate recommendations from the AI service.");
        }

        var courseCodes = aiResponse.Recommendations
            .Select(recommendation => recommendation.CourseCode.Trim())
            .Where(courseCode => !string.IsNullOrWhiteSpace(courseCode))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var courses = await context.Courses
            .AsNoTracking()
            .Where(course => courseCodes.Contains(course.CourseCode))
            .Select(course => new
            {
                course.Id,
                course.CourseCode
            })
            .ToListAsync(cancellationToken);

        var coursesByCode = courses.ToDictionary(
            course => course.CourseCode,
            course => course.Id,
            StringComparer.OrdinalIgnoreCase);

        var recommendationsToAdd = aiResponse.Recommendations
            .DistinctBy(
                recommendation => recommendation.CourseCode,
                StringComparer.OrdinalIgnoreCase)
            .Select(recommendation =>
            {
                var courseCode = recommendation.CourseCode.Trim();

                if (!coursesByCode.TryGetValue(courseCode, out var courseId))
                    return null;

                var difficulty = Enum.TryParse<CourseDifficulty>(
                    recommendation.Difficulty,
                    ignoreCase: true,
                    out var parsedDifficulty)
                        ? parsedDifficulty
                        : CourseDifficulty.Medium;

                return new CourseRecommendation(
                    request.StudentId,
                    courseId,
                    request.SemesterId,
                    difficulty,
                    recommendation.Description,
                    recommendation.Reasoning,
                    recommendation.ExpectedGpaImpact);
            })
            .OfType<CourseRecommendation>()
            .ToList();

        if (recommendationsToAdd.Count is 0)
        {
            return Result<List<CourseRecommendation>>.Error(
                "None of the AI-recommended courses were found in the database.");
        }

        await context.CourseRecommendations.AddRangeAsync(
            recommendationsToAdd,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<List<CourseRecommendation>>.Success(
            recommendationsToAdd,
            "Course recommendations generated successfully.");
    }
}