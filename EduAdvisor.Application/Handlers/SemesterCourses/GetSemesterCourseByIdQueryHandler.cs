using EduAdvisor.Application.DTO.SemesterCourses;
using EduAdvisor.Application.Queries.SemesterCourses;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.SemesterCourses;

public sealed class GetSemesterCourseByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSemesterCourseByIdQuery, Result<SemesterCourseDetailsDto>>
{
    public async Task<Result<SemesterCourseDetailsDto>> Handle(
        GetSemesterCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var sc = await context.SemesterCourses
            .AsNoTracking()
            .Include(x => x.Semester)
            .Include(x => x.CourseAcademicPlan)
                .ThenInclude(p => p.Course)
            .Include(x => x.CourseAcademicPlan)
                .ThenInclude(p => p.Department)
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (sc is null)
            return Result<SemesterCourseDetailsDto>.NotFound("Semester course not found.");

        var dto = new SemesterCourseDetailsDto(
            sc.Id,
            sc.SemesterId,
            sc.Semester.Name,
            sc.Semester.Year,
            sc.Semester.IsRegistrationOpen,
            sc.CourseAcademicPlanId,
            sc.CourseAcademicPlan.Course.CourseCode,
            sc.CourseAcademicPlan.Course.CourseName,
            sc.CourseAcademicPlan.Course.CreditHours,
            sc.CourseAcademicPlan.Course.Type.ToString(),
            sc.CourseAcademicPlan.Level,
            sc.CourseAcademicPlan.StandardSemester,
            sc.CourseAcademicPlan.DepartmentId,
            sc.CourseAcademicPlan.Department?.Name,
            sc.CreatedBy?.UserName,
            sc.CreatedAt,
            sc.UpdatedBy?.UserName,
            sc.UpdatedAt);

        return Result<SemesterCourseDetailsDto>.Success(dto);
    }
}