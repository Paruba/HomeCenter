using FaceRecognitionDotNet;

namespace Boiler.Server.Models.Camera;

public class FileToEncodingModel
{
    public string FileName { get; set; }
    public FaceEncoding Encoding { get; set; }
}
