using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public enum OrganisationType
{
    [Display(Name = "Unknown")] Unknown = 0,

    [Display(Name = "Local authority")] LocalAuthority = 1
}
