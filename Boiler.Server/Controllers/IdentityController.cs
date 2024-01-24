using Boiler.Server.Models.Identity;
using Boiler.Server.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Boiler.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public IdentityController(ILogger<IdentityController> logger, IUserService userService, IRoleService roleService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        try
        {
            var user = await _userService.LoginWithPassword(login);
            var token = _userService.CreateToken(user);
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            Response.Cookies.Append("AccessToken", token, new CookieOptions() { Path = "/", Secure = true });
            return Ok(new { token = token, user = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("Token")]
    [Authorize]
    public async Task<IActionResult> GetToken([FromQuery] string? userId, [FromQuery] string deviceId, [FromQuery] string? device)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var roles = _httpContextAccessor.HttpContext.User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            var token = string.Empty;
            if (roles.Contains("Admin") && userId is not null)
                token = _userService.GenerateDeviceToken(userId, deviceId);
            else
            { 
                var hasAccess = await _userService.HasAccessToDevice(userIdClaim, deviceId, device);
                if (!hasAccess)
                    return Unauthorized();
                token = _userService.GenerateDeviceToken(userIdClaim, deviceId);
            }
            if (!string.IsNullOrEmpty(token))
                return Ok(new { token = token});
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
        return BadRequest();
    }

    [HttpPost("registration")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Registration([FromBody] RegistrationModel registration)
    {
        try
        {
            var user = await _userService.Register(registration);
            if (user != null)
                return Ok();
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("user-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUserRole([FromBody] ApplicationUserRoleLists userRoles)
    {
        try
        {
            await _userService.CreateUserRole(userRoles.Users, userRoles.Roles);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers([FromQuery(Name = "searchValue")] string? searchValue, [FromQuery(Name = "skip")] int? skip,
        [FromQuery(Name = "take")] int? take)
    {
        try
        {
            var users = await _userService.GetUsers(searchValue);
            if (users == null) return NoContent();
            var pagedTemperature = new PageContainer<ApplicationUserModel>
            {
                TotalCount = users.Count(),
                Items = (skip == null || take is null or 0 ? users : users.Skip(skip.Value).Take(take.Value).ToList())
            };
            return Ok(pagedTemperature);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("add-to-roles")]
    [Authorize( Roles = "Admin")]
    public async Task<IActionResult> AddToRoles(ApplicationUserModel user)
    {
        try
        {
            var added = await _userService.AddToRoles(user.Roles.Select(x => x.Id),user.Id);
            if (added) return Ok(added);
            return BadRequest();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserRoles()
    {
        try
        {
            var userRoles = await _userService.GetUserRole();
            return Ok(userRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromQuery(Name = "userId")] string userId)
    {
        try
        {
            await _userService.DeleteUser(userId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user-detail")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUser([FromQuery(Name = "userId")] string userId)
    {
        try
        {
            var user = await _userService.GetUser(userId);
            if (user == null)
                return NoContent();
            return Ok(user);
        } catch (Exception ex) { 
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser([FromBody] UserEditModel user)
    {
        try
        {
            var edited = await _userService.UpdateUser(user);
            if (!edited)
                return Problem();
            return Ok();
        } catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("user-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserRole(string userId, string roleId)
    {
        try
        {
            await _userService.DeleteUserRole(userId, roleId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRegistrationUserDetail([FromQuery(Name = "userId")] string userId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var roles = _httpContextAccessor.HttpContext.User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            if (roles.Contains("Admin"))
                if (string.IsNullOrEmpty(userId))
                    return Ok(await _userService.GetRegistrationDetail(userIdClaim));
                else
                    return Ok(await _userService.GetRegistrationDetail(userId));
            else
                return Ok(await _userService.GetRegistrationDetail(userIdClaim));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("change-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UserChangePassword([FromQuery(Name = "userId")] string? userId, [FromBody] PasswordChangeModel passwordChangeModel)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var roles = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            if (roles.Contains("Admin"))
                if (await _userService.ForceChangePassword(userId, passwordChangeModel.Password))
                    return Ok();
                else
                if (await _userService.ChangePassword(userIdClaim, passwordChangeModel))
                    return Ok();
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("roles")]
    [Authorize( Roles = "Admin")]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var roles = await _roleService.GetRoles();
            return Ok(roles);
        } catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}
