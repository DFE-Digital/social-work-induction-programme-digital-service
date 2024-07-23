using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// Account Type
/// </summary>
public enum AccountType
{
    [Display(Name = "Coordinator")]
    Coordinator,
    [Display(Name = "Assessor")]
    Assessor,
    [Display(Name = "Early career social worker")]
    EarlyCareerSocialWorker,
    [Display(Name = "Assessor, Coordinator")]
    AssessorCoordinator,
}
