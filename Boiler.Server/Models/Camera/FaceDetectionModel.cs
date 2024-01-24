using System.ComponentModel.DataAnnotations.Schema;

namespace Boiler.Server.Models.Camera;

public class FaceDetectionModel
{
    public long Id { get; set; }
    public string? FileName { get; set; }
    public string VideoId { get; set; }
    public virtual VideoModel Video {  get; set; }
    public byte[] Frame { get; set; }
    [NotMapped]
    public string? FrameSrc { get; set; }
    public double Time { get; set; }
    public bool IsFaceRecognized { get; set; } = false;
}
