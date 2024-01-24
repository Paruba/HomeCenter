using Boiler.Server.Hubs;
using Boiler.Server.Models.Camera;
using Boiler.Server.Services.Camera;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Boiler.Server.Controllers;

[ApiController]
[Route("[controller]")]

public class CameraController : ControllerBase
{
    private readonly ILogger<CameraController> _logger;
    private readonly ICameraService _cameraService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHubContext<StreamNotificationHub> _hubContext;

    public CameraController(ILogger<CameraController> logger, ICameraService cameraService,
        IHttpContextAccessor httpContextAccessor, IHubContext<StreamNotificationHub> hubContext)
    {
        _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCameras([FromQuery(Name = "searchValue")] string? searchValue, [FromQuery(Name = "skip")] int? skip,
        [FromQuery(Name = "take")] int? take)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var cameras = await _cameraService.GetCameras(userIdClaim);
            var pagedCameras = new PageContainer<CameraModel>
            {
                TotalCount = cameras.Count(),
                Items = (skip == null || take is null or 0 ? cameras : cameras.Skip(skip.Value).Take(take.Value).ToList())
            };
            return Ok(pagedCameras);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCamera([FromBody] CameraModel camera)
    {
        try { 
        var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        camera.UserId = userIdClaim;
        var isCreated = await _cameraService.CreateCamera(camera);
        if (isCreated)
            return Ok();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
        return BadRequest();
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateCamera([FromBody] CameraModel camera)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (await _cameraService.UpdateCamera(userIdClaim, camera))
                return Ok();
            return BadRequest();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCamera([FromQuery] int cameraId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await _cameraService.DeleteCamera(userIdClaim, cameraId);
            return Ok();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("video")]
    [Authorize]
    public async Task<IActionResult> SaveVideo(IFormFile videoFile)
    {
        try
        {
            var cameraId = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            var video = await _cameraService.CreateVideo(userIdClaim, cameraId, videoFile);
            if (video != null)
                await _hubContext.Clients.User(userIdClaim).SendAsync("ReceiveVideo", video.Name);
            return Ok();

        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("video-upload")]
    [Authorize]
    public async Task<IActionResult> UploadVideo([FromQuery] int cameraId, IFormFile[] video)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await _cameraService.UploadVideo(userIdClaim, cameraId, video);
            return Ok();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("videos")]
    [Authorize]
    public async Task<IActionResult> GetCameraVideos([FromQuery] string cameraId, [FromQuery(Name = "searchValue")] string? searchValue, [FromQuery(Name = "skip")] int? skip,
        [FromQuery(Name = "take")] int? take)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var videos = await _cameraService.GetVideos(cameraId, userIdClaim);
            var pagedVideos = new PageContainer<VideoModel>
            {
                TotalCount = videos.Count(),
                Items = (skip == null || take is null or 0 ? videos : videos.Skip(skip.Value).Take(take.Value).ToList())
            };
            return Ok(pagedVideos);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("video")]
    [Authorize]
    public async Task<IActionResult> GetVideo([FromQuery] string cameraId, [FromQuery] string videoId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var video = await _cameraService.GetVideo(userIdClaim, cameraId, videoId);
            return File(video.FileStream, "video/mp4");
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("video")]
    [Authorize]
    public async Task<IActionResult> DeleteVideo([FromQuery] int cameraId, [FromQuery] string videoId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var deleted = await _cameraService.DeleteVideo(userIdClaim, cameraId, videoId);
            return Ok(deleted);
        } catch (Exception ex)
        {
            _logger?.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("video-last")]
    [Authorize]
    public async Task<IActionResult> GetLastVideo([FromQuery] int cameraId)
    {
        try { 
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var video = await _cameraService.GetLastVideo(userIdClaim, cameraId);
            return File(video.FileStream, "video/mp4");
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("video-name")]
    [Authorize]
    public async Task<IActionResult> GetVideoName([FromQuery] string cameraId, [FromQuery] string videoId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var videoName = await _cameraService.GetVideoName(userIdClaim, cameraId, videoId);
            return Ok(videoName);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("image")]
    [Authorize]
    public async Task<IActionResult> GetImage([FromQuery(Name = "cameraId")] int cameraId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var imageBytes = await _cameraService.GetLastImage(cameraId, userIdClaim);
            if (imageBytes != null)
            {
                string base64Image = Convert.ToBase64String(imageBytes);
                return Ok(base64Image);
            }
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
        return BadRequest();
    }

    [HttpPost("image-stream")]
    [Authorize]
    public async Task<IActionResult> SetImage(IFormFile imageFile)
    {
        try
        {
            var cameraId = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            var imageBytes = await _cameraService.CreateImage(cameraId, userIdClaim, imageFile);
            if (imageBytes != null)
            {
                string base64Image = Convert.ToBase64String(imageBytes);
                await _hubContext.Clients.User(userIdClaim).SendAsync("ReceiveImage", base64Image);
                return Ok();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
        return BadRequest();
    }

    [HttpGet("config")]
    [Authorize]
    public async Task<IActionResult> GetConfig()
    {
        try
        {
            var cameraId = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                    .FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            var config = await _cameraService.GetConfig(userIdClaim, int.Parse(cameraId));
            return Ok(config);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("configuration")]
    [Authorize]
    public async Task<IActionResult> GetCameraConfiguration([FromQuery] int cameraId)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                   .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var config = await _cameraService.GetConfig(userIdClaim, cameraId);
            if (config == null) return NoContent();
            return Ok(config);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }
}
