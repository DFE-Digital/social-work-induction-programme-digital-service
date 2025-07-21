using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
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

    public async Task<Organisation> CreateAsync(Organisation organisation, Account primaryCoordinator)
    {
        if (
            string.IsNullOrWhiteSpace(organisation.OrganisationName)
            || organisation.Type is null
            || organisation.LocalAuthorityCode is null
            || string.IsNullOrWhiteSpace(organisation.Region)
        )
        {
            throw new ArgumentException("Organisation name, Type, Local Authority Code and Region are required");
        }

        if (
            string.IsNullOrWhiteSpace(primaryCoordinator.FirstName)
            || string.IsNullOrWhiteSpace(primaryCoordinator.LastName)
            || string.IsNullOrWhiteSpace(primaryCoordinator.Email)
        )
        {
            throw new ArgumentException("The primary coordinators First name, last name, and email are required");
        }

        var createdOrganisationDto = await authServiceClient.Organisations.CreateAsync(
            new CreateOrganisationRequest
            {
                OrganisationName = organisation.OrganisationName,
                ExternalOrganisationId = organisation.ExternalOrganisationId,
                LocalAuthorityCode = organisation.LocalAuthorityCode,
                Type = organisation.Type,
                PrimaryCoordinatorId = organisation.PrimaryCoordinatorId,
                Region = organisation.Region,
                CreatePersonRequest = new()
                {
                    FirstName = primaryCoordinator.FirstName,
                    LastName = primaryCoordinator.LastName,
                    MiddleName = primaryCoordinator.MiddleNames,
                    EmailAddress = primaryCoordinator.Email,
                    SocialWorkEnglandNumber = primaryCoordinator.SocialWorkEnglandNumber,
                    Roles = primaryCoordinator.Types ?? [],
                    Status = primaryCoordinator.Status,
                    ExternalUserId = primaryCoordinator.ExternalUserId,
                    IsFunded = primaryCoordinator.IsFunded,
                    ProgrammeStartDate = primaryCoordinator.ProgrammeStartDate,
                    ProgrammeEndDate = primaryCoordinator.ProgrammeEndDate
                }
            }
        );

        return mapper.MapToBo(createdOrganisationDto);
    }
}
