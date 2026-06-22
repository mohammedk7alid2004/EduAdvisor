using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.AiRecommendation;
using MediatR;

namespace EduAdvisor.Application.Queries.CourseModules;

public sealed record GetStudentRecommendationsQuery
    : IRequest<Result<StudentRecommendationsResultDto>>;