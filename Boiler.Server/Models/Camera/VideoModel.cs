namespace Boiler.Server.Models.Camera;

public class VideoModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Path { get; set; }
    public int CameraId { get; set; }
    public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();
    public virtual CameraModel Camera { get; set; }
    public bool Processed { get; set; }
}
