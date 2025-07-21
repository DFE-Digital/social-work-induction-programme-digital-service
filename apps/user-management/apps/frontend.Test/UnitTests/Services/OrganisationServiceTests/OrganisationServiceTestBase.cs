using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.OrganisationServiceTests;

public abstract class OrganisationAccountServiceTestBase
{
    private protected OrganisationBuilder OrganisationBuilder { get; }
    private protected AccountBuilder AccountBuilder { get; }

    private protected MockAuthServiceClient MockClient { get; }

    private protected IModelMapper<OrganisationDto, Organisation> Mapper { get; }

    private protected OrganisationService Sut;

    protected OrganisationAccountServiceTestBase()
    {
        OrganisationBuilder = new();
        AccountBuilder = new();
        MockClient = new();
        Mapper = new OrganisationMapper();

        Sut = new(MockClient.Object, Mapper);
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockClient.VerifyNoOtherCalls();
    }
}
