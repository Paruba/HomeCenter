using Boiler.Server.Models.Camera;
using Boiler.Server.Services.Camera;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Boiler.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FaceController : ControllerBase
{
    private readonly ILogger<FaceController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFaceService _faceService;
    public FaceController(ILogger<FaceController> logger, IFaceService faceService,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _faceService = faceService ?? throw new ArgumentNullException(nameof(faceService));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DetectSpecificFaces([FromQuery] int cameraId, [FromQuery] string[] videoIds, IFormFile[] targetFaces)
    {
        try { 
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var detectedFaces = await _faceService.DetectFaces(userIdClaim, cameraId, videoIds);
            if (targetFaces.Length == 0)
                return BadRequest();
            var faces = await _faceService.RecognizeTargetFaces(detectedFaces, targetFaces);
            return Ok(faces);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> DetectFaces([FromQuery] string[] videoIds, [FromQuery] int cameraId)
    {
        try { 
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var faces = await _faceService.DetectFaces(userIdClaim, cameraId, videoIds);
            return Ok(faces);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("detected-faces")]
    [Authorize]
    public async Task<IActionResult> GetFrameWithFace([FromQuery] int cameraId, [FromQuery] string videoId, [FromQuery(Name = "skip")] int? skip,
        [FromQuery(Name = "take")] int? take)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var detectedFaces = await _faceService.GetFrameWithFaces(userIdClaim, cameraId, videoId);
            detectedFaces.ForEach(face => { face.FrameSrc = Convert.ToBase64String(face.Frame); });
            var pagedDetectedFaces = new PageContainer<FaceDetectionModel>
            {
                TotalCount = detectedFaces.Count(),
                Items = (skip == null || take is null or 0 ? detectedFaces : detectedFaces.Skip(skip.Value).Take(take.Value).ToList())
            };
            return Ok(pagedDetectedFaces);
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }
}
