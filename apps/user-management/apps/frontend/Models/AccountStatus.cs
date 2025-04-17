using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// Account Status
/// </summary>
public enum AccountStatus
{
    [Display(Name = "Active")]
    Active,

    [Display(
        Name = "Pending registration",
        Description = "You have not provided a Social Work England registration number for this account"
    )]
    PendingRegistration
}
