using System.ComponentModel.DataAnnotations;

namespace Boiler.Server.Models.Identity;

public class RegistrationModel : LoginModel
{
    [Display(Name = "Kontrola hesla")]
    public string RepeatedPassword { get; set; }
    [Display(Name = "Role")]
    public virtual IEnumerable<string>? Roles { get; set; }
}