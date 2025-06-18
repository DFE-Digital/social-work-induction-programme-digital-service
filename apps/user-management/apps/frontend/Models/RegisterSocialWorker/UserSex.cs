using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

public enum UserSex
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,

    [Display(Name = "Female")] Female,

    [Display(Name = "Male")] Male
}
