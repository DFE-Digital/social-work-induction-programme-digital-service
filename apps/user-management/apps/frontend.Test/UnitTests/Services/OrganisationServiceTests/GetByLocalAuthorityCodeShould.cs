using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Moq;
using Xunit;
using FluentAssertions;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.OrganisationServiceTests;

public class GetByLocalAuthorityCodeShould : OrganisationAccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsMatchingLocalAuthority()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var localAuthorityOrganisation = new Organisation
        {
            LocalAuthorityCode = organisation.LocalAuthorityCode,
            OrganisationName = organisation.OrganisationName,
            Region = organisation.Region,
            Type = OrganisationType.LocalAuthority
        };
        var organisationDto = Mapper.MapFromBo(localAuthorityOrganisation);
        var laCode = localAuthorityOrganisation.LocalAuthorityCode ?? 0;

        MockClient.Setup(x => x.Organisations.GetByLocalAuthorityCodeAsync(laCode)).ReturnsAsync(organisationDto);

        // Act
        var response = await Sut.GetByLocalAuthorityCodeAsync(laCode);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Organisation>();
        response.Should().BeEquivalentTo(localAuthorityOrganisation);

        MockClient.Verify(x => x.Organisations.GetByLocalAuthorityCodeAsync(laCode), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
