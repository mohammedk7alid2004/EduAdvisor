using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Domain.Entities.AcademicModule;
using MediatR;

namespace EduAdvisor.Application.Commands.GenerateRecommendations;

public sealed record GenerateRecommendationsCommand(
    Guid StudentId,
    Guid SemesterId,
    string StudentMajor,
    decimal CurrentGpa,
    int Level,
    int CompletedHours,
    int RegisteredHours,
    int Semester,
    bool IsGraduationSemester,
    List<AvailableCourseDto> AvailableCourses) : IRequest<Result<List<CourseRecommendation>>>;

public sealed record AvailableCourseDto(string CourseCode, List<int> PrereqGrades);