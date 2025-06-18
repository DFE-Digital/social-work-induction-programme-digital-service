using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

public enum EthnicGroupMixed
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,
    [Display(Name = "White and Black Caribbean")] WhiteAndBlackCaribbean,
    [Display(Name = "White and Black African")] WhiteAndBlackAfrican,
    [Display(Name = "White and Asian")] WhiteAndAsian,
    [Display(Name = "Any other mixed or multiple ethnic background")] OtherEthnicGroup,
}
