using Boiler.Server.Models.Camera;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boiler.Server.Services.Camera;

public class CameraService : ICameraService
{
    private readonly ServerDbContext _dbContext;
    private readonly ILogger<CameraService> _logger;
    public CameraService(ServerDbContext dbContext, ILogger<CameraService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> CreateCamera(CameraModel camera)
    {
        try { 
            if (camera == null)
                return false;
            _dbContext.Camera.Add(camera);
            await _dbContext.SaveChangesAsync();
            return true;
        } catch (Exception ex) 
        { 
            _logger.LogError(ex.ToString());
        }
        return false;
    }

    public async Task<bool> UpdateCamera(string userId, CameraModel camera)
    {
        try
        {
            var oldCamera = await _dbContext.Camera.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == camera.Id);
            if (oldCamera == null) return false;
            oldCamera.Period = camera.Period;
            oldCamera.Name = camera.Name;
            _dbContext.Camera.Update(oldCamera);
            await _dbContext.SaveChangesAsync();
            return true;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }

    public async Task<bool> DeleteCamera(string userId, int cameraId)
    {
        try
        {
            var camera = await _dbContext.Camera.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == cameraId);
            if (camera == null) return false;
            _dbContext.Camera.Remove(camera);
            await _dbContext.SaveChangesAsync();
            return true;
        } catch (Exception ex) { 
            _logger?.LogError(ex.ToString());
        }
        return false;
    }
    public async Task<List<CameraModel?>?> GetCameras(string userId)
    {
        try { 
            var cameras = await _dbContext.Camera.Where(x => x.UserId == userId).ToListAsync();
            if (cameras == null) return null;
            return cameras;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<List<VideoModel>> GetVideos(string cameraId, string userId)
    {
        try
        {
            var videoFiles = await _dbContext.Video.Include(x => x.Camera).Where(x => x.CameraId == int.Parse(cameraId) && x.Camera.UserId == userId).ToListAsync();
            return videoFiles;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return new List<VideoModel>();
    }

    public async Task<FileStreamResult?> GetVideo(string userId, string cameraId, string videoId)
    {
        try { 
            var video = await _dbContext.Video.Include(x => x.Camera).FirstOrDefaultAsync(x => x.CameraId == int.Parse(cameraId) && x.Name == videoId && x.Camera.UserId == userId);
            if (video == null)
                return null;
            var stream = new FileStream(video.Path, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, "video/mp4");
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task UploadVideo(string userId, int cameraId, IFormFile[] videoFiles)
    {
        try
        {
            var camera = _dbContext.Camera.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == cameraId);
            if (camera == null) return;
            var userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var targetPath = Path.Combine(userDocumentsPath, "uploaded", userId, cameraId.ToString());

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            foreach (var videoFile in videoFiles) { 
                var filePath = Path.Combine(targetPath, videoFile.FileName);

                var video = new VideoModel()
                {
                    Name = videoFile.FileName,
                    Path = filePath,
                    CameraId = cameraId
                };

                _dbContext.Video.Add(video);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    videoFile.CopyTo(stream);
                }
            }
            await _dbContext.SaveChangesAsync();

        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return;
    }

    public async Task<bool> DeleteVideo(string userId, int cameraId, string videoId)
    {
        try
        {
            var video = await _dbContext.Video.Include(x => x.Camera).FirstOrDefaultAsync(x => x.CameraId == cameraId && x.Name == videoId && x.Camera.UserId == userId);
            if (video == null) return false;
            File.Delete(video.Path);
            _dbContext.Video.Remove(video);
            await _dbContext.SaveChangesAsync();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }

    public async Task<FileStreamResult?> GetLastVideo(string userId, int cameraId)
    {
        try
        {
            var video = await _dbContext.Video.Include(x => x.Camera).OrderByDescending(i => i.Created).FirstOrDefaultAsync(x => x.CameraId == cameraId && x.Camera.UserId == userId);
            var stream = new FileStream(video.Path, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, "video/mp4");
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<string?> GetVideoName(string userId, string cameraId, string videoId)
    {
        try
        {
            var video = await _dbContext.Video.Include(x => x.Camera).FirstOrDefaultAsync(x => x.CameraId == int.Parse(cameraId) && x.Name == videoId && x.Camera.UserId == userId);
            if (video == null)
                return null;
            return video.Name;
        } catch (Exception ex) { 
            _logger.LogError(ex.ToString());
        }
        return null;
    } 

    public async Task<VideoModel?> CreateVideo(string userId, string cameraId, IFormFile videoFile)
    {
        try
        {
            var userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var targetPath = Path.Combine(userDocumentsPath, "uploaded", userId, cameraId);

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var filePath = Path.Combine(targetPath, videoFile.FileName);

            var video = new VideoModel()
            {
                Name = videoFile.FileName,
                Path = filePath,
                CameraId = int.Parse(cameraId)
            };

            _dbContext.Video.Add(video);

            await _dbContext.SaveChangesAsync();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                videoFile.CopyTo(stream);
            }
            return video;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError($"Access denied: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<byte[]?> GetLastImage(int cameraId, string userId)
    {
        try
        {
            var lastImage = await _dbContext.Image.Include(x => x.Camera).OrderByDescending(i => i.CreatedDate).FirstOrDefaultAsync(x => x.CameraId == cameraId && x.Camera.UserId == userId);
            if (lastImage == null) return null;
            return lastImage.ImageData;
        } catch (Exception ex)
        {
            _logger?.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<byte[]?> CreateImage(string cameraId,string userId, IFormFile videoFile)
    {
        try { 
            using var memoryStream = new MemoryStream();

            await videoFile.CopyToAsync(memoryStream);

            var lastImage = await _dbContext.Image.Include(x => x.Camera).OrderByDescending(i => i.CreatedDate).FirstOrDefaultAsync(x => x.CameraId == int.Parse(cameraId) && x.Camera.UserId == userId);
            if (lastImage != null)
                _dbContext.Image.Remove(lastImage);

            _dbContext.Image.Add(new ImageModel()
            {
                Name = videoFile.FileName,
                ImageData = memoryStream.ToArray(),
                CameraId = int.Parse(cameraId)
            });

            await _dbContext.SaveChangesAsync();
            return memoryStream.ToArray();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<CameraModel?> GetConfig(string userId, int cameraId)
    {
        try { 
            var config = await _dbContext.Camera.FirstOrDefaultAsync(x => x.Id == cameraId && x.UserId == userId);
            return config;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }
}
