using Boiler.Server.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Boiler.Server.Models.Thermometer;

public class ThermometerModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set;} = Guid.NewGuid().ToString();
    public string Name { get; set;}
    public string? UserId { get; set; }
    public virtual ApplicationUserModel? User { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<TemperatureModel?>? Temperature { get; set;}
}
