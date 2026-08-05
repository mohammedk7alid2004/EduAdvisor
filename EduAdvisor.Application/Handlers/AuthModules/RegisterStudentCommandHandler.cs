using EduAdvisor.Application.Commands.AuthModules;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.User;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Application.Interfaces.File;
using EduAdvisor.Domain.Entities.AuthModule;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Application.Handlers.AuthModules;

public sealed class RegisterStudentCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext context,
    IStringLocalizer localizer,
    IOtpService otpService,
    IBackgroundJobClient backgroundJobClient,
    IFileStorageService fileStorage)
    : IRequestHandler<RegisterStudentCommand, Result<UserResponseDto>>
{
    public async Task<Result<UserResponseDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate Email

        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result<UserResponseDto>.Failure(
                localizer["UserAlreadyExists"],
                400);

        #endregion

        #region Validate Department

        var departmentExists = await context.Departments
            .AnyAsync(x => x.Id == request.DepartmentId, cancellationToken);

        if (!departmentExists)
            return Result<UserResponseDto>.Failure(
                localizer["DepartmentNotFound"],
                404);

        #endregion

        #region Validate Student Code

        var studentCodeExists = await context.Students
            .AnyAsync(x => x.StudentCode == request.StudentCode, cancellationToken);

        if (studentCodeExists)
            return Result<UserResponseDto>.Failure(
                localizer["StudentCodeAlreadyExists"],
                400);

        #endregion

        #region Upload Image

        string? profileImageUrl = null;

        if (request.ProfileImage is not null)
        {
            var uniqueFileName =
                $"{Guid.NewGuid()}{Path.GetExtension(request.ProfileImage.FileName)}";

            profileImageUrl = await fileStorage.SaveFileAsync(
                request.ProfileImage.OpenReadStream(),
                uniqueFileName,
                "profile-images");

            if (profileImageUrl is null)
                return Result<UserResponseDto>.Failure(
                    localizer["ImageUploadFailed"],
                    400);
        }

        #endregion

        #region Create User

        var fullName =
            $"{request.FirstName.Trim()} {request.LastName.Trim()}";

        var user = new User(
            fullName,
            request.Email,
            request.NationalId)
        {
            EmailConfirmed = false
        };

        user.SetProfileImage(profileImageUrl);

        var createResult = await userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
        {
            if (profileImageUrl is not null)
                await fileStorage.DeleteFileAsync(profileImageUrl);

            return Result<UserResponseDto>.Failure(
                $"{localizer["UserRegistrationFailed"]}: {createResult.Errors.First().Description}",
                400);
        }

        #endregion

        #region Assign Role

        var roleResult = await userManager.AddToRoleAsync(
            user,
            "Student");

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            if (profileImageUrl is not null)
                await fileStorage.DeleteFileAsync(profileImageUrl);

            return Result<UserResponseDto>.Failure(
                $"{localizer["UserRegistrationFailed"]}: {roleResult.Errors.First().Description}",
                400);
        }

        #endregion

        #region Create Student

        try
        {
            var student = new Domain.Entities.AuthModule.Student(
                user.Id,
                request.StudentCode,
                request.DepartmentId);

            await context.Students.AddAsync(
                student,
                cancellationToken);

            await context.SaveChangesAsync(
                cancellationToken);
        }
        catch
        {
            await userManager.DeleteAsync(user);

            if (profileImageUrl is not null)
                await fileStorage.DeleteFileAsync(profileImageUrl);

            throw;
        }

        #endregion

        #region Send OTP

        var otp = await otpService.GenerateAndStoreAsync(
            request.Email,
            OtpType.EmailConfirmation,
            cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(
            service => service.SendConfirmationEmail(user, otp));

        #endregion

        var response = new UserResponseDto(
            user.Id,
            user.FullName,
            user.Email!,
            user.EmailConfirmed);

        return Result<UserResponseDto>.Success(
            response,
            localizer["UserRegisteredSuccessfully"],
            201);
    }
}