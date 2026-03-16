using System.ComponentModel.DataAnnotations;

namespace StratSphere.Web.Models.ViewModels.Account;

public class ProfileViewModel
{
    [Required]
    [Display(Name = "Display Name")]
    [StringLength(100, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    public DateTime MemberSince { get; set; }

    public int LeagueCount { get; set; }
}
