using Boiler.Server.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Boiler.Server.Models.Camera;

public class CameraModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Period { get; set; }
    public string? UserId { get; set; }
    public virtual ApplicationUserModel? User { get; set; }
}
