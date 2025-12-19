using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;
using ArgumentException = System.ArgumentException;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.OrganisationServiceTests;

public class UpdateShould : OrganisationAccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CallsAuthService_ReturnsOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var organisationDto = Mapper.MapFromBo(organisation);
        var updateRequest = new UpdateOrganisationRequest
        {
            OrganisationId = organisation.OrganisationId ?? Guid.Empty,
            OrganisationName = organisation.OrganisationName,
            ExternalOrganisationId = organisation.ExternalOrganisationId,
            LocalAuthorityCode = organisation.LocalAuthorityCode,
            Type = organisation.Type,
            PrimaryCoordinatorId = organisation.PrimaryCoordinatorId,
            Region = organisation.Region,
            PhoneNumber = organisation.PhoneNumber
        };

        MockClient
            .Setup(x => x.Organisations.UpdateOrganisationAsync(MoqHelpers.ShouldBeEquivalentTo(updateRequest)))
            .ReturnsAsync(organisationDto);

        // Act
        var response = await Sut.UpdateOrganisationAsync(organisation);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Organisation>();
        response.Should().BeEquivalentTo(organisation);
        MockClient.Verify(
            x => x.Organisations.UpdateOrganisationAsync(MoqHelpers.ShouldBeEquivalentTo(updateRequest)),
            Times.Once
        );

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledWithNullData_ThrowsArgumentException()
    {
        // Arrange
        var organisationToUpdate = new Organisation
        {
            OrganisationId = Guid.Empty,
            OrganisationName = string.Empty,
            LocalAuthorityCode = null,
            Region = string.Empty
        };

        // Act
        var actualException = await Assert.ThrowsAsync<ArgumentException>(
            async () => await Sut.UpdateOrganisationAsync(organisationToUpdate)
        );

        // Assert
        actualException.Should().BeOfType<ArgumentException>();
        actualException
            .Message.Should()
            .Be("Organisation name, Type, Local Authority Code, Region and Id are required");

        VerifyAllNoOtherCalls();
    }
}
