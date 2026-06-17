using EduAdvisor.Application.DTO.CourseAcademicPlans;
using MediatR;

namespace EduAdvisor.Application.Queries.CourseAcademicPlans;

public sealed record GetMyCoursesQuery
    : IRequest<Result<MyCoursesResponseDto>>;