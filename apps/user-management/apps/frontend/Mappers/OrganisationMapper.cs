using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Mappers;

public class OrganisationMapper : IModelMapper<OrganisationDto, Organisation>
{
    public Organisation MapToBo(OrganisationDto organisationDto)
    {
        return new Organisation
        {
            OrganisationId = organisationDto.OrganisationId,
            OrganisationName = organisationDto.OrganisationName,
            ExternalOrganisationId = organisationDto.ExternalOrganisationId,
            LocalAuthorityCode = organisationDto.LocalAuthorityCode,
            Type = organisationDto.Type,
            PrimaryCoordinatorId = organisationDto.PrimaryCoordinatorId,
            Region = organisationDto.Region
        };
    }

    public OrganisationDto MapFromBo(Organisation organisation)
    {
        return new OrganisationDto
        {
            OrganisationId = organisation.OrganisationId,
            OrganisationName = organisation.OrganisationName,
            ExternalOrganisationId = organisation.ExternalOrganisationId,
            LocalAuthorityCode = organisation.LocalAuthorityCode,
            Type = organisation.Type,
            PrimaryCoordinatorId = organisation.PrimaryCoordinatorId,
            Region = organisation.Region
        };
    }
}
