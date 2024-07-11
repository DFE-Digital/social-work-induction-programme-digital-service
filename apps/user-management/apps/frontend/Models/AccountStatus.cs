using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

public enum AccountStatus
{
    [Display(Name = "Active")]
    Active,
    [Display(Name = "Inactive")]
    Inactive,
    [Display(Name = "Pending Registration", Description = "You have not provided a Social Work England registration number for this account")]
    PendingRegistration,
    [Display(Name = "Paused", Description = "Taking a break from the PQP programme")]
    Paused,
}
