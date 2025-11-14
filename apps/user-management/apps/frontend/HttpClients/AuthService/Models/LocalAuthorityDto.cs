using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

public class LocalAuthorityDto
{
    public required string OrganisationName { get; set; }
    public required int LocalAuthorityCode { get; set; }
    public required string Region { get; set; }

    public Organisation ToOrganisation()
    {
        return new Organisation()
        {
            OrganisationName = OrganisationName,
            LocalAuthorityCode = LocalAuthorityCode,
            Region = Region,
            Type = OrganisationType.LocalAuthority
        };
    }
}
