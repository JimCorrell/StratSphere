using System.ComponentModel.DataAnnotations;

namespace Stratsphere.Web.Models.ViewModels.League;

public class CreateLeagueViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
