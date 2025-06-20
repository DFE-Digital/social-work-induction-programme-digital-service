using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;

public enum EthnicGroupWhite
{
    [Display(Name = "Prefer not to say")] PreferNotToSay,
    [Display(Name = "English, Welsh, Scottish, Northern Irish or British")] EnglishWelshScottishNorthernIrishOrBritish,
    [Display(Name = "Irish")] Irish,
    [Display(Name = "Gypsy or Irish Traveller")] GypsyOrIrishTraveller,
    [Display(Name = "Any other White background")] AnyOtherWhiteBackground,
    [Display(Name = "Other ethnic group")] OtherEthnicGroup,
}
