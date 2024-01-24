using Boiler.Server.Framework;
using Boiler.Server.Models.Identity;

namespace Boiler.Server.Services.Identity;

public interface IUserService : IDependency
{
    Task<ApplicationUserModel?> LoginWithPassword(LoginModel login);
    Task<ApplicationUserModel?> Register(RegistrationModel registration);
    Task CreateUserRole(List<ApplicationUserModel> users, List<ApplicationRoleModel> roles);
    Task<List<ApplicationUserModel>?> GetUsers(string? searchValue);
    Task<bool> AddToRoles(IEnumerable<string> roles, string userId);
    Task<List<ApplicationUserRoleModel>?> GetUserRole();
    Task<bool> UpdateUser(UserEditModel user);
    Task DeleteUser(string userId);
    Task<ApplicationUserModel?> GetUser(string userId);
    Task DeleteUserRole(string userId, string roleId);
    Task<RegistrationModel?> GetRegistrationDetail(string userId);
    Task<bool> ForceChangePassword(string userId, string password);
    Task<bool> ChangePassword(string userId, PasswordChangeModel model);
    Task<bool> HasAccessToDevice(string userId, string deviceId, string? device);
    string CreateToken(ApplicationUserModel user);
    string GenerateDeviceToken(string userId, string deviceId);
}
