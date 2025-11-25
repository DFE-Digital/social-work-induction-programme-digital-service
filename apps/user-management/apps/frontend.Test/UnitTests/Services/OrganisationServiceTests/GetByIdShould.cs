using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Moq;
using Xunit;
using FluentAssertions;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.OrganisationServiceTests;

public class GetByIdShould : OrganisationAccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsMatchingOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var organisationDto = Mapper.MapFromBo(organisation);
        var id = organisation.OrganisationId ?? Guid.Empty;

        MockClient.Setup(x => x.Organisations.GetByIdAsync(id)).ReturnsAsync(organisationDto);

        // Act
        var response = await Sut.GetByIdAsync(id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Organisation>();
        response.Should().BeEquivalentTo(organisation);

        MockClient.Verify(x => x.Organisations.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenNoOrganisationFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockClient.Setup(x => x.Organisations.GetByIdAsync(id)).ReturnsAsync((OrganisationDto?)null);

        // Act
        var response = await Sut.GetByIdAsync(id);

        // Assert
        response.Should().BeNull();

        MockClient.Verify(x => x.Organisations.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
