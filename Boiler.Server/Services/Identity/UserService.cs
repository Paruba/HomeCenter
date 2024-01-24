using Boiler.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Boiler.Server.Services.Identity;

public class UserService : IUserService
{
    private readonly ServerDbContext _dbContext;
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public UserService(ServerDbContext dbContext, ILogger<UserService> logger, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    public async Task<ApplicationUserModel?> LoginWithPassword(LoginModel login)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == login.Username.ToLower());

            if (user == null)
                return null;
            else if (!await _userManager.CheckPasswordAsync(user, login.Password))
                return null;
            else
            {
                var appUser = await _dbContext.ApplicationUsers.Include(x => x.Roles).FirstOrDefaultAsync(x => x.UserName.ToLower() == user.UserName.ToLower());
                return appUser;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<ApplicationUserModel?> Register(RegistrationModel registration)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == registration.Username.ToLower());
            if (user != null)
                return null;
            if (registration.Password.Length < 5 || registration.Password != registration.RepeatedPassword)
                return null;

            var createdUser = new IdentityUser
            {
                Email = registration.Username,
                UserName = registration.Username,
                EmailConfirmed = true,
            };
            var createUserResult = await _userManager.CreateAsync(createdUser, registration.Password);
            if (!createUserResult.Succeeded)
                return null;

            var roles = await _dbContext.Roles.Where(x => registration.Roles.Contains(x.Id)).ToListAsync();

            if (registration.Roles.Any() && roles.Any())
                await _userManager.AddToRolesAsync(createdUser, roles.Select(x => x.Name));
            else
                await _userManager.AddToRoleAsync(createdUser, "Initializer");

            var appUser = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName.ToLower() == registration.Username.ToLower());
            await _dbContext.SaveChangesAsync();
            return appUser;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<bool> AddToRoles(IEnumerable<string> roles, string userId)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return false;
            var result = await _userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }


    public async Task CreateUserRole(List<ApplicationUserModel> users, List<ApplicationRoleModel> roles)
    {
        try
        {
            var identityUsers = await _dbContext.Users.Where(x => users.Select(x => x.Id).Contains(x.Id)).ToListAsync();
            if (identityUsers.Any() && roles.Any())
            {
                foreach (var user in identityUsers)
                {
                    await _userManager.AddToRolesAsync(user, roles.Select(x => x.Name));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    public async Task<List<ApplicationUserModel>?> GetUsers(string? searchValue = null)
    {
        try
        {
            var userQuery = _dbContext.ApplicationUsers.AsQueryable();
            if (!string.IsNullOrEmpty(searchValue))
                userQuery = userQuery.Where(x => x.UserName.Contains(searchValue));
            var users = await userQuery.ToListAsync();
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public async Task<List<ApplicationUserRoleModel>?> GetUserRole()
    {
        try
        {
            var userRoles = await _dbContext.ApplicationUserRoles
                .Include(x => x.User)
                .Include(x => x.Role)
                .ToListAsync();
            return userRoles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public async Task DeleteUser(string userId)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    public async Task<ApplicationUserModel?> GetUser(string userId)
    {
        try
        {
            var user = await _dbContext.ApplicationUsers.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == userId);
            return user;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<bool> UpdateUser(UserEditModel user)
    {
        try
        {
            var userRoleQuery = _dbContext.UserRoles.Where(x => x.UserId == user.Id);
            var oldUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            var roleIds = await userRoleQuery.Select(x => x.RoleId).ToListAsync();
            var roles = await _dbContext.Roles.ToListAsync();
            var oldRoles = await _dbContext.Roles.Where(x => roleIds.Contains(x.Id)).ToListAsync();
            if (oldUser != null && oldRoles != null && oldRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(oldUser, oldRoles.Select(x => x.Id));
                await _userManager.AddToRolesAsync(oldUser, roles.Where(x => user.Roles.Contains(x.Id)).Select(x => x.Name));
                await _dbContext.SaveChangesAsync();
                return true;
            }
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }

    public async Task DeleteUserRole(string userId, string roleId)
    {
        try
        {
            var userRole = await _dbContext.ApplicationUserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
            if (userRole != null)
            {
                _dbContext.ApplicationUserRoles.Remove(userRole);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    public async Task<RegistrationModel?> GetRegistrationDetail(string userId)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
                return new RegistrationModel()
                {
                    Username = user.UserName
                };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<bool> ForceChangePassword(string userId, string password)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, password);
                return result.Succeeded;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }

    public async Task<bool> ChangePassword(string userId, PasswordChangeModel model)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
                return result.Succeeded;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }

    public string CreateToken(ApplicationUserModel user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name.ToString()));
        }
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
            .GetBytes(_configuration.GetSection(Constants.Auth.Token).Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public async Task<bool> HasAccessToDevice(string userId, string deviceId, string? device)
    {
        switch (device)
        {
            case "thermometer":
                var thermometer = await _dbContext.Thermometer.FirstOrDefaultAsync(x => x.Id == deviceId && x.UserId == userId);
                if (thermometer != null) return true;
                break;
            default:
                var camera = await _dbContext.Camera.FirstOrDefaultAsync(x => x.Id == int.Parse(deviceId) && x.UserId == userId);
                if (camera != null) return true;
                break;
        }

        return false;
    }

    public string GenerateDeviceToken(string userId, string deviceId)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, deviceId),
            new Claim(ClaimTypes.Name, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
            .GetBytes(_configuration.GetSection(Constants.Auth.Token).Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
