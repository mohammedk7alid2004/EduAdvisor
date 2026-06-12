
using EduAdvisor.Application.DTO.User;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.Users;

public sealed class GetAdvisorStudentsQueryHandler(
    IApplicationDbContext db,
    IGetCurrentUserRepository currentUser)
    : IRequestHandler<GetAdvisorStudentsQuery, Result<PaginatedList<AdvisorStudentResponse>>>
{
    #region Handle

    public async Task<Result<PaginatedList<AdvisorStudentResponse>>> Handle(
        GetAdvisorStudentsQuery request,
        CancellationToken cancellationToken)
    {
        #region Get Current Advisor

        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Result<PaginatedList<AdvisorStudentResponse>>
                .Unauthorized("Unauthorized.");

        var advisor = await db.Advisors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (advisor is null)
            return Result<PaginatedList<AdvisorStudentResponse>>
                .NotFound("Advisor not found.");

        //if (advisor.IsPending)
        //    return Result<PaginatedList<AdvisorStudentResponse>>
        //        .Forbidden("Advisor is not approved yet.");

        #endregion

        #region Build Query

        var query = db.Students
            .AsNoTracking()
            .Where(x => x.AdvisorId == advisor.Id);

        #endregion

        #region Filters

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.User.Email!.Contains(request.Search) ||
                x.StudentCode.Contains(request.Search) ||
                x.User.FullName.Contains(request.Search));

        #endregion

        #region Projection

        var projected = query
            .OrderBy(x => x.User.FullName)
            .Select(x => new AdvisorStudentResponse(
                x.Id,
                x.StudentCode,
                x.User.FullName,
                x.User.Email!,
                x.GPA,
                x.AcademicYear));

        #endregion

        #region Paginate

        var result = await PaginatedList<AdvisorStudentResponse>
            .CreateAsync(projected, request.PageNumber, request.PageSize);

        #endregion

        return Result<PaginatedList<AdvisorStudentResponse>>.Success(result);
    }

    #endregion
}