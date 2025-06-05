using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// User Status
/// </summary>
public enum UserStatus
{
    [Display(Name = "Active")]
    Active,

    [Display(Name = "Pending")]
    PendingRegistration
}
