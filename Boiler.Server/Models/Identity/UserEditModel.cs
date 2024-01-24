namespace Boiler.Server.Models.Identity;

public class UserEditModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
