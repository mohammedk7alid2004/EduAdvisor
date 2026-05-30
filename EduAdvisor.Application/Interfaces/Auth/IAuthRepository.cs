using EduAdvisor.Application.DTO.Auth;

namespace EduAdvisor.Application.Interfaces.Auth
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailOrPhoneAsync(string target);
        Task<RoleDto?> GetRoleByNameAsync(string roleName);
        Task<List<PermissionDTO>> GetPermissionsByRoleIdAsync(string roleId);
        Task<List<string>> GetAllPermissionNamesAsync();
        Task UpdateUserAsync(User user);
    }
}
