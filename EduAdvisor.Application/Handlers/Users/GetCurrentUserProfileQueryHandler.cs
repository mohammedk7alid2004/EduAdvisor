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

public sealed class GetCurrentUserProfileQueryHandler(
    IApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager,
    IStringLocalizer localizer)
    : IRequestHandler<GetCurrentUserProfileQuery, Result<CurrentUserResponseDTO>>
{
    private const string StudentRole = "Student";
    private const string AdvisorRole = "Advisor";

    public async Task<Result<CurrentUserResponseDTO>> Handle(
        GetCurrentUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<CurrentUserResponseDTO>.Failure(
                localizer["Unauthorized"],
                StatusCodes.Status401Unauthorized);
        }

        var user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                user => user.Id == userId,
                cancellationToken);

        if (user is null)
        {
            return Result<CurrentUserResponseDTO>.Failure(
                localizer["UserNotFound"],
                StatusCodes.Status404NotFound);
        }

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var studentProfile = role == StudentRole
            ? await GetStudentProfileAsync(userId, cancellationToken)
            : null;

        var advisorProfile = role == AdvisorRole
            ? await GetAdvisorProfileAsync(userId, cancellationToken)
            : null;

        var response = new CurrentUserResponseDTO(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.EmailConfirmed,
            user.CreatedAt,
            role,
            studentProfile,
            advisorProfile);

        return Result<CurrentUserResponseDTO>.Success(
            response,
            localizer["OperationCompletedSuccessfully"]);
    }

    private Task<StudentProfileDto?> GetStudentProfileAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return context.Students
            .AsNoTracking()
            .Where(student => student.UserId == userId)
            .Select(student => new StudentProfileDto(
                student.StudentCode,
                student.Department.Name,
                student.GPA,
                student.CompletedHours,
                student.AcademicYear,
                student.Advisor != null
                    ? student.Advisor.User.FullName
                    : null))
            .SingleOrDefaultAsync(cancellationToken);
    }

    private Task<AdvisorProfileDto?> GetAdvisorProfileAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return context.Advisors
            .AsNoTracking()
            .Where(advisor => advisor.UserId == userId)
            .Select(advisor => new AdvisorProfileDto(
                advisor.Department.Name,
                advisor.IsPending,
                advisor.Students.Count,
                context.Enrollments.Count(enrollment =>
                    enrollment.ReviewedByAdvisorId == advisor.Id &&
                    enrollment.Status == EnrollmentStatus.Pending)))
            .SingleOrDefaultAsync(cancellationToken);
    }
}