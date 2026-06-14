using System.Security.Claims;
using EduAdvisor.Application.DTO.User;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.Users;
using EduAdvisor.Domain.Enums.University;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.Users;

public class GetCurrentUserProfileQueryHandler(
    IApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager,
    IStringLocalizer localizer)
    : IRequestHandler<GetCurrentUserProfileQuery, Result<CurrentUserResponseDTO>>
{
    public async Task<Result<CurrentUserResponseDTO>> Handle(
        GetCurrentUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        #region Get Current User Id

        var userId = httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Result<CurrentUserResponseDTO>.Failure(localizer["Unauthorized"], 401);

        #endregion

        #region Fetch User

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result<CurrentUserResponseDTO>.Failure(localizer["UserNotFound"], 404);

        #endregion

        #region Get Role

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        #endregion

        #region Build Profile Based On Role

        StudentProfileDto? studentProfile = null;
        AdvisorProfileDto? advisorProfile = null;

        if (role == "Student")
        {
            var student = await context.Students
                .AsNoTracking()
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (student is not null)
            {
                studentProfile = new StudentProfileDto(
                    student.StudentCode,
                    student.Department.Name,
                    student.GPA,
                    student.CompletedHours,
                    student.AcademicYear);
            }
        }
        else if (role == "Advisor")
        {
            var advisor = await context.Advisors
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.Students)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (advisor is not null)
            {
                var pendingRequestsCount = await context.Enrollments
                    .CountAsync(x =>
                        x.ReviewedByAdvisorId == advisor.Id &&
                        x.Status == EnrollmentStatus.Pending,
                        cancellationToken);

                advisorProfile = new AdvisorProfileDto(
                    advisor.Department.Name,
                    advisor.IsPending,
                    advisor.Students.Count,
                    pendingRequestsCount);
            }
        }

        #endregion

        #region Build Response

        var dto = new CurrentUserResponseDTO(
            user.Id,
            user.FullName,
            user.Email!,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.EmailConfirmed,
            user.CreatedAt,
            role,
            studentProfile,
            advisorProfile);

        #endregion

        return Result<CurrentUserResponseDTO>.Success(dto, localizer["OperationCompletedSuccessfully"]);
    }
}