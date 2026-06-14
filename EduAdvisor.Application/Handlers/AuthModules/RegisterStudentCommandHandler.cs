using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.DTO.User;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Application.Interfaces.File;
using EduAdvisor.Domain.Entities.AuthModule;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public class RegisterStudentCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext context,
    IStringLocalizer localizer,
    IMemoryCache memoryCache,
    IEmailService emailService,
    IHasherService hasher,
    IFileStorageService fileStorage)
    : IRequestHandler<RegisterStudentCommand, Result<UserResponseDto>>
{
    public async Task<Result<UserResponseDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate Email Uniqueness

        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result<UserResponseDto>.Failure(localizer["UserAlreadyExists"], 400);

        #endregion

        #region Validate Department

        var departmentExists = await context.Departments
            .AnyAsync(x => x.Id == request.DepartmentId, cancellationToken);

        if (!departmentExists)
            return Result<UserResponseDto>.Failure(localizer["DepartmentNotFound"], 404);

        #endregion

        #region Validate Student Code Uniqueness

        var studentCodeExists = await context.Students
            .AnyAsync(x => x.StudentCode == request.StudentCode, cancellationToken);

        if (studentCodeExists)
            return Result<UserResponseDto>.Failure(localizer["StudentCodeAlreadyExists"], 400);

        #endregion

        #region Upload Profile Image

        string? profileImageUrl = null;

        if (request.ProfileImage is not null)
        {
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfileImage.FileName)}";
            profileImageUrl = await fileStorage.SaveFileAsync(
                request.ProfileImage.OpenReadStream(),
                uniqueFileName,
                "profile-images");

            if (profileImageUrl is null)
                return Result<UserResponseDto>.Failure(localizer["ImageUploadFailed"], 400);
        }

        #endregion

        #region Create User

        var fullName = $"{request.FirstName.Trim()} {request.LastName.Trim()}";

        var user = new User(fullName, request.Email, request.NationalId)
        {
            EmailConfirmed = false,
        };

        user.SetProfileImage(profileImageUrl);

        var createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            await fileStorage.DeleteFileAsync(profileImageUrl);
            var error = createResult.Errors.Select(e => e.Description).First();
            return Result<UserResponseDto>.Failure(localizer["UserRegistrationFailed"] + ": " + error, 400);
        }

        #endregion

        #region Assign Role

        var roleResult = await userManager.AddToRoleAsync(user, "Student");

        if (!roleResult.Succeeded)
        {
            await fileStorage.DeleteFileAsync(profileImageUrl);
            var error = roleResult.Errors.Select(e => e.Description).First();
            return Result<UserResponseDto>.Failure(localizer["UserRegistrationFailed"] + ": " + error, 400);
        }

        #endregion

        #region Create Student Profile

        var student = new Domain.Entities.AuthModule.Student(user.Id, request.StudentCode, request.DepartmentId);
        await context.Students.AddAsync(student, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        #endregion

        #region Send OTP

        var otp = new Random().Next(100000, 999999).ToString();
        memoryCache.Set($"EmailOTP_{request.Email}", hasher.Hash(otp), TimeSpan.FromMinutes(5));
        await emailService.SendConfirmationEmail(user, otp);

        #endregion

        var userResponse = new UserResponseDto(user.Id, user.FullName, user.Email!, user.EmailConfirmed);
        return Result<UserResponseDto>.Success(userResponse, localizer["UserRegisteredSuccessfully"], 201);
    }
}