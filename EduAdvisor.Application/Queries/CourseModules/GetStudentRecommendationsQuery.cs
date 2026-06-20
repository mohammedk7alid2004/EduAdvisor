using EduAdvisor.Application.Commands.GenerateRecommendations;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.AiRecommendation;
using MediatR;

namespace EduAdvisor.Application.Queries.CourseModules;

public sealed record GetStudentRecommendationsQuery(
    Guid StudentId,
    Guid SemesterId,
    string StudentMajor,
    decimal CurrentGpa,
    int Level,
    int CompletedHours,
    int RegisteredHours,
    int Semester,
    bool IsGraduationSemester,
    List<AvailableCourseDto> AvailableCourses)
    : IRequest<Result<List<StudentRecommendationDto>>>;