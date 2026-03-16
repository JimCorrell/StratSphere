using System.ComponentModel.DataAnnotations;

namespace StratSphere.Web.Models.ViewModels.Season;

public class CreateSeasonViewModel
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(1871, 2025, ErrorMessage = "Card year must be between 1871 and 2025.")]
    public int CardYear { get; set; } = DateTime.UtcNow.Year - 1;

    public string LeagueAbbreviation { get; set; } = string.Empty;
}
