using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class GetOrganisationShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_ReturnsOrganisation()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var expectedOrganisation = OrganisationBuilder.WithPrimaryCoordinatorId(account.Id).Build();

        MockOrganisationService.Setup(x => x.GetByIdAsync(expectedOrganisation.OrganisationId!.Value)).ReturnsAsync(expectedOrganisation);
        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var response = await Sut.GetOrganisationAsync(expectedOrganisation.OrganisationId!.Value);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedOrganisation);
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = await Sut.GetOrganisationAsync(Guid.Empty);

        // Assert
        response.Should().BeNull();
    }
}
