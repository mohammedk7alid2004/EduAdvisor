using EduAdvisor.Application.DTO.User;
using Microsoft.AspNetCore.Http;

namespace EduAdvisor.Application.Commands.AuthModules;

public record RegisterAdvisorCommand(
    string FirstName,
    string LastName,
    string Email,
    Guid DepartmentId,
    string NationalId,
    string Phone,
    string Password,
    string ConfirmPassword,
    IFormFile? ProfileImage
) : IRequest<Result<UserResponseDto>>;