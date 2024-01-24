using Boiler.Server.Framework;
using Boiler.Server.Models.Camera;

namespace Boiler.Server.Services.Camera;

public interface IFaceService : IDependency
{
    Task<List<FaceDetectionModel>> DetectFaces(string userId, int cameraId, string[] videoIds);
    List<FaceDetectionModel> DetectFaces(VideoModel video);
    Task<List<FaceDetectionModel>> RecognizeTargetFaces(List<FaceDetectionModel> faces, IFormFile[] targetFaces);
    Task<List<FaceDetectionModel>> GetFrameWithFaces(string userId, int cameraId, string videoId);
}
