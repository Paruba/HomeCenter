using Boiler.Server.Framework;
using Boiler.Server.Models.Identity;

namespace Boiler.Server.Services.Identity;

public interface IRoleService : IDependency
{
    Task<List<ApplicationRoleModel>?> GetRoles(string? searchValue = null);
    Task<bool> CreateRole(string name);
    Task<bool> Delete(string roleId);
}
