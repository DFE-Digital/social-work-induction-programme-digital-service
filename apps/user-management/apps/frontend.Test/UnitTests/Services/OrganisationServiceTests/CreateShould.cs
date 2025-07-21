using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
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
        var primaryCoordinator = AccountBuilder.Build();
        var organisation = OrganisationBuilder.Build();
        var organisationDto = Mapper.MapFromBo(organisation);

        var createRequest = new CreateOrganisationRequest
        {
            OrganisationName = organisation.OrganisationName!,
            ExternalOrganisationId = organisation.ExternalOrganisationId,
            LocalAuthorityCode = organisation.LocalAuthorityCode,
            Type = organisation.Type,
            PrimaryCoordinatorId = organisation.PrimaryCoordinatorId,
            Region = organisation.Region,
            CreatePersonRequest = new CreatePersonRequest
            {
                FirstName = primaryCoordinator.FirstName!,
                LastName = primaryCoordinator.LastName!,
                MiddleName = primaryCoordinator.MiddleNames,
                EmailAddress = primaryCoordinator.Email!,
                SocialWorkEnglandNumber = primaryCoordinator.SocialWorkEnglandNumber,
                Roles = primaryCoordinator.Types ?? [],
                Status = primaryCoordinator.Status,
                ExternalUserId = primaryCoordinator.ExternalUserId,
                IsFunded = primaryCoordinator.IsFunded,
                ProgrammeStartDate = primaryCoordinator.ProgrammeStartDate,
                ProgrammeEndDate = primaryCoordinator.ProgrammeEndDate
            }
        };

        MockClient
            .Setup(x => x.Organisations.CreateAsync(MoqHelpers.ShouldBeEquivalentTo(createRequest)))
            .ReturnsAsync(organisationDto);

        // Act
        var response = await Sut.CreateAsync(organisation, primaryCoordinator);

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
        var account = new Account();

        // Act & Assert
        await FluentActions.Awaiting(() => Sut.CreateAsync(organisation, account))
            .Should().ThrowAsync<ArgumentException>();
    }
}
