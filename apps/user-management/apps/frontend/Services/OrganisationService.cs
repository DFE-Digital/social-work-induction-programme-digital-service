using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class OrganisationService(
    IAuthServiceClient authServiceClient,
    IModelMapper<OrganisationDto, Organisation> mapper
) : IOrganisationService
{
    public async Task<PaginationResult<Organisation>> GetAllAsync(PaginationRequest request)
    {
        var organisationDto = await authServiceClient.Organisations.GetAllAsync(request);

        var organisations = new PaginationResult<Organisation>
        {
            Records = organisationDto.Records.Select(mapper.MapToBo).ToList(),
            MetaData = organisationDto.MetaData
        };

        return organisations;
    }

    // TODO Get from database when data is available
    public Organisation GetByLocalAuthorityCode(int? localAuthorityCode)
    {
        return new Organisation
        {
            OrganisationId = new Guid(),
            LocalAuthorityCode = localAuthorityCode,
            OrganisationName = "Test Organisation",
            Type = OrganisationType.LocalAuthority,
            Region = "Test Region"
        };
    }
}
