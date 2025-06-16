using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

public enum EthnicGroupOther
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,
    [Display(Name = "Arab")] Arab,
    [Display(Name = "Any other ethnic group")] AnyOtherEthnicGroup,
}
