using System.ComponentModel.DataAnnotations;

namespace StratSphere.Web.Models.ViewModels.League;

public class AssignCommissionerViewModel
{
    [Required]
    public Guid NewCommissionerId { get; set; }
}
