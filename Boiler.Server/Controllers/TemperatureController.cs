using Boiler.Server.Hubs;
using Boiler.Server.Models;
using Boiler.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Boiler.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TemperatureController : ControllerBase
{
    private readonly ILogger<TemperatureController> _logger;
    private readonly ITemperatureService _temperatureService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHubContext<TemperatureHub> _temperatureHubContext;
    public TemperatureController(ILogger<TemperatureController> logger, ITemperatureService temperatureService, IHttpContextAccessor httpContextAccessor, IHubContext<TemperatureHub> temperatureHub)
    {
        _temperatureService = temperatureService ?? throw new ArgumentNullException(nameof(temperatureService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _temperatureHubContext = temperatureHub ?? throw new ArgumentNullException(nameof(temperatureHub));
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTemperatures([FromQuery(Name = "skip")] int? skip,
        [FromQuery(Name = "take")] int? take, [FromQuery(Name = "thermometerId")] string? thermometerId)
    {
        try {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var temperatures = await _temperatureService.GetTemperatures(userIdClaim, thermometerId);
            if (temperatures == null) return NoContent();
            var pagedTemperature = new PageContainer<TemperatureModel>
            {
                TotalCount = temperatures.Count(),
                Items = (skip == null || take is null or 0 ? temperatures : temperatures.Skip(skip.Value).Take(take.Value).ToList())
            };
            return Ok(pagedTemperature);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetTemperature([FromQuery(Name = "thermometerId")] string? thermometerId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentTemperature = await _temperatureService.GetCurrentTemperature(userIdClaim, thermometerId);
            return Ok(currentTemperature);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SetCurrent([FromBody] TemperatureModel temperature)
    {
        try
        {
            var thermometerId = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            temperature.ThermometerId = thermometerId;
            if (double.Parse(temperature.Value) > 25) { 
                var result = await _temperatureService.SetTemperature(temperature);
                if (result) {
                    await _temperatureHubContext.Clients.User(userIdClaim).SendAsync("ReceiveTemperature", temperature);
                    return Ok();
                }
            }
            return NoContent();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }
}
