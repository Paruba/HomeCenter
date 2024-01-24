using Boiler.Server.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Buffers;

namespace Boiler.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    private readonly ILogger<RoleController> _logger;
    private readonly IRoleService _roleService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public RoleController(ILogger<RoleController> logger, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoles(string? searchValue)
    {
        try
        {
            var roles = await _roleService.GetRoles(searchValue);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        try
        {
            var role = await _roleService.CreateRole(roleName);
            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        try
        {
            var isDeleted = await _roleService.Delete(roleId);
            if (isDeleted)
                return Ok();
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }
}
