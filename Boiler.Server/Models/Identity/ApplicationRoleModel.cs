using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Boiler.Server.Models.Identity;

public class ApplicationRoleModel
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<ApplicationUserModel> Users { get; set; }
}