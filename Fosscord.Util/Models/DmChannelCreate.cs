using System.ComponentModel.DataAnnotations;
using Fosscord.DbModel.Scaffold;

namespace Fosscord.Util.Models;

public class DmChannelCreate
{
    [Required]
    public string name { get; set; }
    
    [Required]
    public string[] recipients { get; set; }
}