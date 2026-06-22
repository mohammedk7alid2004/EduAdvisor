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

        var aiRequest = CreateAiRequest(request);

        var aiResponse =
            await aiRecommendationService.GetRecommendationsAsync(
                aiRequest,
                cancellationToken);

        if (aiResponse?.Recommendations is not { Count: > 0 })
        {
            return Result<List<CourseRecommendation>>.Error(
                "Unable to generate recommendations from the AI service.");
        }

        var validRecommendations = aiResponse.Recommendations
            .Where(recommendation =>
                !string.IsNullOrWhiteSpace(recommendation.CourseCode))
            .DistinctBy(
                recommendation => recommendation.CourseCode.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (validRecommendations.Count is 0)
        {
            return Result<List<CourseRecommendation>>.Error(
                "The AI service returned invalid course codes.");
        }

        var courseCodes = validRecommendations
            .Select(recommendation => recommendation.CourseCode.Trim())
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

        var coursesByCode = courses
            .Where(course =>
                !string.IsNullOrWhiteSpace(course.CourseCode))
            .GroupBy(
                course => course.CourseCode.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.First().Id,
                StringComparer.OrdinalIgnoreCase);

        var recommendationsToAdd = validRecommendations
            .Select(recommendation =>
                CreateRecommendation(
                    recommendation,
                    request,
                    coursesByCode))
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

    private static AiRecommendationRequestDto CreateAiRequest(
        GenerateRecommendationsCommand request)
    {
        return new AiRecommendationRequestDto
        {
            StudentMajor = request.StudentMajor,
            CurrentGpa = request.CurrentGpa,
            Level = request.Level,
            CompletedHours = request.CompletedHours,
            RegisteredHours = request.RegisteredHours,
            Semester = request.Semester,
            IsGraduationSemester = request.IsGraduationSemester,

            AvailableCourses = request.AvailableCourses
                .Where(course =>
                    !string.IsNullOrWhiteSpace(course.CourseCode))
                .Select(course => new AiAvailableCourseDto
                {
                    CourseCode = course.CourseCode.Trim(),
                    PrereqGrades = course.PrereqGrades
                })
                .ToList()
        };
    }

    private static CourseRecommendation? CreateRecommendation(
        AiCourseRecommendationDto recommendation,
        GenerateRecommendationsCommand request,
        IReadOnlyDictionary<string, Guid> coursesByCode)
    {
        var courseCode = recommendation.CourseCode.Trim();

        if (!coursesByCode.TryGetValue(courseCode, out var courseId))
            return null;

        var difficulty = ParseDifficulty(
            recommendation.DifficultyLabel);

        var description = recommendation.Advice.Verdict;

        var reasoning = recommendation.Advice.Reasons.Count > 0
            ? string.Join(
                Environment.NewLine,
                recommendation.Advice.Reasons)
            : "No reasoning was provided by the AI service.";

        var expectedGpaImpact =
            recommendation.PredictedGpa - request.CurrentGpa;

        return new CourseRecommendation(
            request.StudentId,
            courseId,
            request.SemesterId,
            difficulty,
            description,
            reasoning,
            expectedGpaImpact);
    }

    private static CourseDifficulty ParseDifficulty(
        string difficultyLabel)
    {
        return Enum.TryParse<CourseDifficulty>(
            difficultyLabel,
            ignoreCase: true,
            out var difficulty)
                ? difficulty
                : CourseDifficulty.Medium;
    }
}