using System.ComponentModel.DataAnnotations;

namespace StratSphere.Web.Models.ViewModels.League;

public class CreateLeagueViewModel
{
    [Required, MaxLength(100)]
    [Display(Name = "League Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "League Abbreviation")]
    [StringLength(6, MinimumLength = 4)]
    [RegularExpression(@"^[A-Z0-9]{4,6}$", ErrorMessage = "Abbreviation must be 4–6 uppercase letters or digits (no spaces or special characters).")]
    public string Abbreviation { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>Set by the controller before returning the view when the user has no phone on record.</summary>
    public bool ShowPhoneField { get; set; }
}
