using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

public enum AccountType
{
    [Display(Name = "Coordinator")]
    Coordinator,
    [Display(Name = "Assessor")]
    Assessor,
    [Display(Name = "Early career social worker")]
    EarlyCareerSocialWorker,
}
