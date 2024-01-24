using Boiler.Server.Framework;
using Boiler.Server.Models.Camera;
using Microsoft.AspNetCore.Mvc;

namespace Boiler.Server.Services.Camera;

public interface ICameraService : IDependency
{
    Task<List<CameraModel?>?> GetCameras(string userId);
    Task<bool> CreateCamera(CameraModel camera);
    Task<bool> UpdateCamera(string userId, CameraModel camera);
    Task<bool> DeleteCamera(string userId, int cameraId);
    Task<List<VideoModel>> GetVideos(string cameraId, string userId);
    Task<FileStreamResult?> GetVideo(string userId, string cameraId, string videoId);
    Task<bool> DeleteVideo(string userId, int cameraId, string videoId);
    Task<FileStreamResult?> GetLastVideo(string userId, int cameraId);
    Task<string?> GetVideoName(string userId, string cameraId, string videoId);
    Task<VideoModel?> CreateVideo(string userId, string cameraId, IFormFile videoFile);
    Task UploadVideo(string userId, int cameraId, IFormFile[] videoFiles);
    Task<byte[]?> CreateImage(string cameraId, string userId, IFormFile videoFile);
    Task<byte[]?> GetLastImage(int cameraId, string userId);
    Task<CameraModel?> GetConfig(string userId, int cameraId);
}
