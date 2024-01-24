using Boiler.Server.Models;
using Boiler.Server.Models.Thermometer;
using Boiler.Server.Services;
using Boiler.Server.Services.Thermometer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Boiler.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ThermometerController : ControllerBase
{
    private readonly ILogger<ThermometerController> _logger;
    private readonly IThermometerService _thermometerService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ThermometerController(ILogger<ThermometerController> logger, IThermometerService thermometerService, IHttpContextAccessor httpContextAccessor)
    {
        _thermometerService = thermometerService ?? throw new ArgumentNullException(nameof(thermometerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetThermometers([FromQuery(Name = "searchValue")]string? searchValue,[FromQuery(Name = "skip")] int? skip,
        [FromQuery(Name = "take")] int? take)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var thermometers = await _thermometerService.GetThermometers(userIdClaim);
            var pagedThermometers = new PageContainer<ThermometerModel>
            {
                TotalCount = thermometers.Count(),
                Items = (skip == null || take is null or 0 ? thermometers : thermometers.Skip(skip.Value).Take(take.Value).ToList())
            };
            return Ok(pagedThermometers);
        } catch (Exception ex) { 
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateThermometer(ThermometerModel thermometer)
    {
        try { 
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            thermometer.UserId = userIdClaim;
            var isCreated = await _thermometerService.CreateThermometer(thermometer);
            if (isCreated)
                return Ok();
            else return BadRequest();
        } catch (Exception ex)
        {
            _logger?.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteThermometer([FromQuery(Name = "thermometerId")] string thermometerId)
    {
        try { 
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var isDeleted = await _thermometerService.DeleteThermometer(thermometerId, userIdClaim);
            return Ok();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }
}
