using System.ComponentModel.DataAnnotations;

namespace Stratsphere.Web.Models.ViewModels.League;

public class JoinLeagueViewModel
{
    [Required]
    public string Slug { get; set; } = string.Empty;
}
