using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

public enum EthnicGroup
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,
    [Display(Name = "White")] White,
    [Display(Name = "Mixed or multiple ethnic groups")] MixedOrMultipleEthnicGroups,
    [Display(Name = "Asian or Asian British")] AsianOrAsianBritish,
    [Display(Name = "Black, African, Caribbean or Black British")] BlackAfricanCaribbeanOrBlackBritish,
    [Display(Name = "Other ethnic group")] OtherEthnicGroup,
}
