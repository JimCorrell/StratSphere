using System.ComponentModel.DataAnnotations;

namespace StratSphere.Web.Models.ViewModels.League;

public class JoinLeagueViewModel
{
    [Required]
    public string Slug { get; set; } = string.Empty;
}
