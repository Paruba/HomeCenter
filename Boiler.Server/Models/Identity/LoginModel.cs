using System.ComponentModel.DataAnnotations;

namespace Boiler.Server.Models.Identity;

public class LoginModel
{
    [Display(Name = "Email")]
    public string Username { get; set; }
    [Display(Name = "Heslo")]
    public string Password { get; set; }
}