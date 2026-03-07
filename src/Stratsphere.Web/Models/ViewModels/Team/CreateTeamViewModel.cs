using System.ComponentModel.DataAnnotations;

namespace StratSphere.Web.Models.ViewModels.Team;

public class CreateTeamViewModel
{
    [Required, MaxLength(100)]
    public string City { get; set; } = "";

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [Required, MaxLength(10)]
    public string Abbreviation { get; set; } = "";
}
