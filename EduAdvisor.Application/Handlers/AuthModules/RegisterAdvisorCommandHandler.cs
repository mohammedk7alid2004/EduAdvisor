using EduAdvisor.Application.Commands.AuthModules;
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

public sealed class RegisterAdvisorCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext context,
    IStringLocalizer localizer,
    IFileStorageService fileStorage,
    IOtpService otpService,
    IBackgroundJobClient backgroundJobClient)
    : IRequestHandler<RegisterAdvisorCommand, Result<UserResponseDto>>
{
    public async Task<Result<UserResponseDto>> Handle(
        RegisterAdvisorCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate Email Uniqueness

        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result<UserResponseDto>.Failure(
                localizer["UserAlreadyExists"],
                400);

        #endregion

        #region Validate Department

        var departmentExists = await context.Departments
            .AnyAsync(
                x => x.Id == request.DepartmentId,
                cancellationToken);

        if (!departmentExists)
            return Result<UserResponseDto>.Failure(
                localizer["DepartmentNotFound"],
                404);

        #endregion

        #region Upload Profile Image

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
            PhoneNumber = request.Phone,
            EmailConfirmed = false
        };

        user.SetProfileImage(profileImageUrl);

        var createResult =
            await userManager.CreateAsync(
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

        var roleResult =
            await userManager.AddToRoleAsync(
                user,
                "Advisor");

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

        #region Create Advisor Profile

        try
        {
            var advisor = new Advisor(
                user.Id,
                request.DepartmentId);

            await context.Advisors.AddAsync(
                advisor,
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

        #region Generate OTP

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