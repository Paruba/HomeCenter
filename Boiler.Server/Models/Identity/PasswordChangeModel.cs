using System.ComponentModel.DataAnnotations;

namespace Boiler.Server.Models.Identity;

public class PasswordChangeModel : RegistrationModel
{
    [Display(Name = "Současné heslo")]
    public string? OldPassword { get; set; }
}