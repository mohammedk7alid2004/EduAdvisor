using EduAdvisor.Application.DTO.Semesters;
using EduAdvisor.Application.Queries.Semesters;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Semesters;

public sealed class GetSemesterByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSemesterByIdQuery, Result<SemesterDetailsDto>>
{
    public async Task<Result<SemesterDetailsDto>> Handle(
        GetSemesterByIdQuery request,
        CancellationToken cancellationToken)
    {
        var semester = await context.Semesters
            .AsNoTracking()
            .Include(s => s.CreatedBy)
            .Include(s => s.UpdatedBy)
            .FirstOrDefaultAsync(s => s.Id == request.SemesterId, cancellationToken);

        if (semester is null)
            return Result<SemesterDetailsDto>.NotFound("Semester not found.");

        var dto = new SemesterDetailsDto(
            semester.Id,
            semester.Name,
            semester.Year,
            semester.StartDate,
            semester.EndDate,
            semester.IsActive,
            semester.IsRegistrationOpen,
            semester.StandardSemesterNumber,
            semester.IsCurrentDateInSemester(),
            semester.CreatedBy?.UserName,
            semester.CreatedAt,
            semester.UpdatedBy?.UserName,
            semester.UpdatedAt);

        return Result<SemesterDetailsDto>.Success(dto);
    }
}