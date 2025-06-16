using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

public enum EthnicGroupBlack
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,
    [Display(Name = "African")] African,
    [Display(Name = "Caribbean")] Caribbean,
    [Display(Name = "Any other Black, African or Caribbean background")] AnyOtherBlackAfricanOrCaribbeanBackground,
}
