﻿using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

public enum GenderMatchesSexAtBirth
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,

    [Display(Name = "Yes")] Yes,

    [Display(Name = "No")] No
}
