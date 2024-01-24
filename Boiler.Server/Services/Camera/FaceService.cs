using Microsoft.EntityFrameworkCore;
using Boiler.Server.Models.Camera;
using DlibDotNet;
using OpenCvSharp;
using System.Runtime.InteropServices;
using FaceRecognitionDotNet;
using OpenCvSharp.Extensions;
using System.Drawing;
using Image = FaceRecognitionDotNet.Image;

namespace Boiler.Server.Services.Camera;

public class FaceService : IFaceService
{
    private readonly ServerDbContext _dbContext;
    private readonly ILogger<FaceService> _logger;
    public FaceService(ServerDbContext dbContext, ILogger<FaceService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<FaceDetectionModel>> DetectFaces(string userId, int cameraId, string[] videoIds)
    {
        var videos = await _dbContext.Video.Include(x => x.Camera)
                .Where(x => videoIds.Contains(x.Name) && x.Camera.UserId == userId && x.CameraId == cameraId).ToListAsync();

        var detectedRecords = await _dbContext.Faces.Where(x => videos.Select(x => x.Id).Contains(x.VideoId)).ToListAsync();
        _dbContext.Faces.RemoveRange(detectedRecords);

        List<FaceDetectionModel> detectedFaces = new List<FaceDetectionModel>();

        foreach (var video in videos)
        {
            detectedFaces.AddRange(DetectFaces(video));
            video.Processed = true;
            _dbContext.Video.Update(video);
        }
        await _dbContext.SaveChangesAsync();
        return detectedFaces;
    }

    public List<FaceDetectionModel> DetectFaces(VideoModel video)
    {
        List<FaceDetectionModel> detectedFaces = new List<FaceDetectionModel>();
        var times = new List<double>();
        var cap = new VideoCapture(video.Path);
        if (!cap.IsOpened())
        {
            _logger.LogError("Unable to connect to camera");
            return detectedFaces;
        }
        int frameCounter = 0;
        //using (var win = new ImageWindow())
        //{
            using (var detector = Dlib.GetFrontalFaceDetector())
            using (var poseModel = ShapePredictor.Deserialize("shape_predictor_68_face_landmarks.dat"))
            {
                while (true) // !win.IsClosed()
            {
                    var temp = new Mat();
                    if (!cap.Read(temp))
                    {
                        break;
                    }
                    var array = new byte[temp.Width * temp.Height * temp.ElemSize()];
                    Marshal.Copy(temp.Data, array, 0, array.Length);

                    frameCounter++;

                    using (var cimg = Dlib.LoadImageData<BgrPixel>(array, (uint)temp.Height, (uint)temp.Width, (uint)(temp.Width * temp.ElemSize())))
                    {
                        var faces = detector.Operator(cimg);
                        var shapes = new List<FullObjectDetection>();
                        for (var i = 0; i < faces.Length; ++i)
                        {
                            var det = poseModel.Detect(cimg, faces[i]);
                            shapes.Add(det);
                            double time = cap.Get(VideoCaptureProperties.PosMsec);
                            detectedFaces.Add(new FaceDetectionModel()
                            {
                                VideoId = video.Id,
                                Frame = ConvertBitmapToByteArray(BitmapConverter.ToBitmap(temp)),
                                Time = time,
                                FileName = ""
                            });
                            Console.WriteLine($"Face detected at: {time} ms");
                        }

                        // win.ClearOverlay();
                        // win.SetImage(cimg);
                        // var lines = Dlib.RenderFaceDetections(shapes);
                        // win.AddOverlay(lines);
                        // 
                        // foreach (var line in lines)
                        //     line.Dispose();
                    }
                }
            }
        //}
        return detectedFaces;
    }

    public async Task<List<FaceDetectionModel>> RecognizeTargetFaces(List<FaceDetectionModel> faces, IFormFile[] targetFaces)
    {
        var tempFilePaths = new List<string>();
        List<FileToEncodingModel> targetEncodings = new List<FileToEncodingModel>();
        var modelParameter = new ModelParameter
        {
            PosePredictor68FaceLandmarksModel = File.ReadAllBytes("shape_predictor_68_face_landmarks.dat"),
            PosePredictor5FaceLandmarksModel = File.ReadAllBytes("shape_predictor_5_face_landmarks.dat"),
            FaceRecognitionModel = File.ReadAllBytes("dlib_face_recognition_resnet_model_v1.dat"),
            CnnFaceDetectorModel = File.ReadAllBytes("mmod_human_face_detector.dat")
        };

        try {
            FaceRecognition fr = FaceRecognition.Create(modelParameter);
            foreach (var file in targetFaces)
            {
                var tempFilePath = Path.GetTempFileName();
                tempFilePaths.Add(tempFilePath);

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                Image imageTarget = FaceRecognition.LoadImageFile(tempFilePath);
                IEnumerable<Location> locationTarget = fr.FaceLocations(imageTarget);
                if (locationTarget.Any()) { 
                    IEnumerable<FaceEncoding> encodingTarget = fr.FaceEncodings(imageTarget, locationTarget);
                    foreach (var encoding in encodingTarget)
                    {
                        targetEncodings.Add(new FileToEncodingModel(){
                            Encoding = encoding,
                            FileName = file.FileName
                        });
                    }
                }
            }

            foreach (var face in faces)
            {
                var bitmap = ConvertByteArrayToBitmap(face.Frame);
                Image detectedImage = FaceRecognition.LoadImage(bitmap);
                IEnumerable<Location> datectedLocation = fr.FaceLocations(detectedImage);
                if (datectedLocation.Any())
                {
                    IEnumerable<FaceEncoding> detectedEncoding = fr.FaceEncodings(detectedImage, datectedLocation);
                    foreach (var targetEncoding in targetEncodings)
                    {
                        const double tolerance = 0.6d;
                        bool match = FaceRecognition.CompareFace(targetEncoding.Encoding, detectedEncoding.First(), tolerance);
                        if (match)
                        {
                            face.FileName = targetEncoding.FileName;
                            face.IsFaceRecognized = true;
                        }
                    }
                }
            }

            await _dbContext.Faces.AddRangeAsync(faces);
            await _dbContext.SaveChangesAsync();
        } catch (Exception ex) {
            _logger.LogError(ex.ToString());
        } finally
        {
            foreach (var path in tempFilePaths)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
        }
        
        return faces;
    }

    public async Task<List<FaceDetectionModel>> GetFrameWithFaces(string userId, int cameraId, string videoId)
    {
        try
        {
            var detectedFaces = await _dbContext.Faces.Include(x => x.Video).ThenInclude(x => x.Camera)
            .Where(x => x.VideoId == videoId && x.Video.CameraId == cameraId && x.Video.Camera.UserId == userId && x.IsFaceRecognized).ToListAsync();
            return detectedFaces;
        } catch (Exception ex)
        {
            _logger?.LogError(ex.ToString());
        }

        return new List<FaceDetectionModel>();
    }

    private byte[] ConvertBitmapToByteArray(System.Drawing.Bitmap image)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }

    private Bitmap ConvertByteArrayToBitmap(byte[] imageBytes)
    {
        using (MemoryStream stream = new MemoryStream(imageBytes))
        {
            return new Bitmap(stream);
        }
    }
}
