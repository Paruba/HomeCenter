using Boiler.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Boiler.Server.Services.Identity;

public class RoleService : IRoleService
{
    private readonly ServerDbContext _dbContext;
    private readonly ILogger<RoleService> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    public RoleService(ServerDbContext dbContext, ILogger<RoleService> logger, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public async Task<List<ApplicationRoleModel>?> GetRoles(string? searchValue = null)
    {
        try
        {
            var query = _dbContext.ApplicationRoles.AsQueryable();
            if (!string.IsNullOrEmpty(searchValue))
                query = query.Where(x => x.Name.Contains(searchValue));
            var roles = await query.ToListAsync();
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<bool> CreateRole(string name)
    {
        if (!await _roleManager.RoleExistsAsync(name))
        {
            var role = new IdentityRole() { Name = name };
            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded;
        }
        return false;
    }

    public async Task<bool> Delete(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        return false;
    }
}
