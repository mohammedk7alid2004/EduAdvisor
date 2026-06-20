using EduAdvisor.Application.Commands.GenerateRecommendations;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.AiRecommendation;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.CourseModules;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.GenerateRecommendations;

public sealed class GetStudentRecommendationsQueryHandler(
    IApplicationDbContext context,
    ISender sender)
    : IRequestHandler<
        GetStudentRecommendationsQuery,
        Result<List<StudentRecommendationDto>>>
{
    public async Task<Result<List<StudentRecommendationDto>>> Handle(
        GetStudentRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        var studentExists = await context.Students
            .AsNoTracking()
            .AnyAsync(
                student => student.Id == request.StudentId,
                cancellationToken);

        if (!studentExists)
        {
            return Result<List<StudentRecommendationDto>>
                .NotFound("Student not found.");
        }

        var recommendations = await GetRecommendationsAsync(
            request.StudentId,
            request.SemesterId,
            cancellationToken);

        if (recommendations.Count is 0)
        {
            var generateCommand = new GenerateRecommendationsCommand(
                StudentId: request.StudentId,
                SemesterId: request.SemesterId,
                StudentMajor: request.StudentMajor,
                CurrentGpa: request.CurrentGpa,
                Level: request.Level,
                CompletedHours: request.CompletedHours,
                RegisteredHours: request.RegisteredHours,
                Semester: request.Semester,
                IsGraduationSemester: request.IsGraduationSemester,
                AvailableCourses: request.AvailableCourses);

            var generationResult = await sender.Send(
                generateCommand,
                cancellationToken);

            if (!generationResult.IsSuccess)
            {
                return Result<List<StudentRecommendationDto>>.Error(
                    generationResult.Message);
            }

            recommendations = await GetRecommendationsAsync(
                request.StudentId,
                request.SemesterId,
                cancellationToken);
        }

        return Result<List<StudentRecommendationDto>>.Success(
            recommendations,
            "Student recommendations retrieved successfully.");
    }

    private Task<List<StudentRecommendationDto>> GetRecommendationsAsync(
        Guid studentId,
        Guid semesterId,
        CancellationToken cancellationToken)
    {
        return context.CourseRecommendations
            .AsNoTracking()
            .Where(recommendation =>
                recommendation.StudentId == studentId &&
                recommendation.SemesterId == semesterId)
            .Join(
                context.Courses.AsNoTracking(),
                recommendation => recommendation.CourseId,
                course => course.Id,
                (recommendation, course) =>
                    new StudentRecommendationDto
                    {
                        Id = recommendation.Id,
                        CourseId = course.Id,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        CreditHours = course.CreditHours,
                        SemesterId = recommendation.SemesterId,
                        Difficulty =
                            recommendation.Difficulty.ToString(),
                        Description = recommendation.Description,
                        Reasoning = recommendation.Reasoning,
                        ExpectedGpaImpact =
                            recommendation.ExpectedGpaImpact
                    })
            .OrderBy(recommendation => recommendation.CourseCode)
            .ToListAsync(cancellationToken);
    }
}