using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.SemesterCourses;
using EduAdvisor.Application.Queries.SemesterCourses;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.SemesterCourses;

public sealed class GetAllSemesterCoursesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllSemesterCoursesQuery, Result<PaginatedList<SemesterCourseListDto>>>
{
    public async Task<Result<PaginatedList<SemesterCourseListDto>>> Handle(
        GetAllSemesterCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.SemesterCourses
            .AsNoTracking()
            .AsQueryable();

        if (request.SemesterId.HasValue)
            query = query.Where(sc => sc.SemesterId == request.SemesterId.Value);

        if (request.DepartmentId.HasValue)
            query = query.Where(sc => sc.CourseAcademicPlan.DepartmentId == request.DepartmentId.Value);

        if (request.Level.HasValue)
            query = query.Where(sc => sc.CourseAcademicPlan.Level == request.Level.Value);

        if (request.StandardSemester.HasValue)
            query = query.Where(sc => sc.CourseAcademicPlan.StandardSemester == request.StandardSemester.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(sc =>
                sc.CourseAcademicPlan.Course.CourseName.ToLower().Contains(search) ||
                sc.CourseAcademicPlan.Course.CourseCode.ToLower().Contains(search) ||
                sc.Semester.Name.ToLower().Contains(search));
        }

        var groupedQuery = query
            .GroupBy(sc => new
            {
                sc.SemesterId,
                sc.Semester.Name,
                sc.Semester.Year
            })
            .Select(g => new SemesterCourseListDto(
                g.Key.SemesterId,
                g.Key.Name,
                g.Key.Year,
                g.Select(sc => new CourseItemDto(
                    sc.Id,
                    sc.CourseAcademicPlanId,
                    sc.CourseAcademicPlan.Course.CourseCode,
                    sc.CourseAcademicPlan.Course.CourseName,
                    sc.CourseAcademicPlan.Course.CreditHours,
                    sc.CourseAcademicPlan.Level,
                    sc.CourseAcademicPlan.StandardSemester,
                    sc.CourseAcademicPlan.Department != null ? sc.CourseAcademicPlan.Department.Name : null,
                    sc.CreatedBy != null ? sc.CreatedBy.UserName : null,
                    sc.CreatedAt
                )).ToList()
            ));

        var result = await PaginatedList<SemesterCourseListDto>.CreateAsync(
            groupedQuery,
            request.PageNumber,
            request.PageSize
        );

        return Result<PaginatedList<SemesterCourseListDto>>.Success(result);
    }
}