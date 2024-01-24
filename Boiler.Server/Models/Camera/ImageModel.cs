namespace Boiler.Server.Models.Camera;

public class ImageModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public byte[] ImageData { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now.ToUniversalTime();
    public int CameraId { get; set; }
    public virtual CameraModel Camera { get; set; }
}
