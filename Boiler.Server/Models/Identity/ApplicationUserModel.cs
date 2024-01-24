using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Boiler.Server.Models.Identity;

public class ApplicationUserModel
{
    [Key]
    public string Id { get; set; }
    public string UserName { get; set; }
    public virtual IEnumerable<ApplicationRoleModel> Roles { get; set; }
}