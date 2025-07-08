using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;

public enum EthnicGroupAsian
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,
    [Display(Name = "Indian")] Indian,
    [Display(Name = "Pakistani")] Pakistani,
    [Display(Name = "Bangladeshi")] Bangladeshi,
    [Display(Name = "Chinese")] Chinese,
    [Display(Name = "Any other Asian background")] AnyOtherAsianBackground,
}
