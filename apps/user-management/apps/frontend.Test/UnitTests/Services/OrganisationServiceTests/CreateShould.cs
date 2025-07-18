using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.OrganisationServiceTests;

public class CreateShould : OrganisationAccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsCreatedOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var organisationDto = Mapper.MapFromBo(organisation);

        var createRequest = new CreateOrganisationRequest
        {
            OrganisationName = organisation.OrganisationName!,
            ExternalOrganisationId = organisation.ExternalOrganisationId,
            LocalAuthorityCode = organisation.LocalAuthorityCode,
            Type = organisation.Type,
            PrimaryCoordinatorId = organisation.PrimaryCoordinatorId,
            Region = organisation.Region
        };

        MockClient
            .Setup(x => x.Organisations.CreateAsync(MoqHelpers.ShouldBeEquivalentTo(createRequest)))
            .ReturnsAsync(organisationDto);

        // Act
        var response = await Sut.CreateAsync(organisation);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Organisation>();
        response.Should().BeEquivalentTo(organisation);

        MockClient.Verify(
            x => x.Organisations.CreateAsync(MoqHelpers.ShouldBeEquivalentTo(createRequest)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledWithNullValues_ThrowArgumentException()
    {
        // Arrange
        var organisation = new Organisation();

        // Act & Assert
        await FluentActions.Awaiting(() => Sut.CreateAsync(organisation))
            .Should().ThrowAsync<ArgumentException>();
    }
}
