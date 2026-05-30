using EduAdvisor.Application.DTO.User;
using Microsoft.AspNetCore.Http;

namespace EduAdvisor.Application.Commands.AuthModules;

public record RegisterStudentCommand(
    string FirstName,
    string LastName,
    string Email,
    string StudentCode,
    Guid DepartmentId,
    string NationalId,
    string Password,
    string ConfirmPassword,
    IFormFile? ProfileImage
) : IRequest<Result<UserResponseDto>>;