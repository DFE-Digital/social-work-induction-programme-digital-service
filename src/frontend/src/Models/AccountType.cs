using System.ComponentModel.DataAnnotations;

namespace SocialWorkInductionProgramme.Frontend.Models;

/// <summary>
/// Account Type
/// </summary>
public enum AccountType
{
    [Display(Name = "Coordinator")]
    Coordinator = 800,

    [Display(Name = "Assessor")]
    Assessor = 600,

    [Display(Name = "Early career social worker")]
    EarlyCareerSocialWorker = 400
}
