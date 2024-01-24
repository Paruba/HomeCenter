namespace Boiler.Server.Models.Identity;


public class ApplicationUserRoleModel
{
    public string UserId { get; set; }
    public virtual ApplicationUserModel User { get; set; }
    public string RoleId { get; set; }
    public virtual ApplicationRoleModel Role { get; set; }
}