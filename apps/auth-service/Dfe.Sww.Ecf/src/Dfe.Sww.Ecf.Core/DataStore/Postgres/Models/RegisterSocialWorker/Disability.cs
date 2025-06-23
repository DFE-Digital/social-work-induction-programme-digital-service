using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;

public enum Disability
{
    [Display(Name = "Yes")] Yes,

    [Display(Name = "No")] No,

    [Display(Name = "Prefer not to say")] PreferNotToSay
}
