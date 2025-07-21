using System.Collections.Immutable;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using Faker;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers;

public class OrganisationsControllerTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    [Fact]
    public async Task GetAllAsync_ReturnsOkResult_WhenOrganisationsExist()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            const int expectedCount = 5;
            var expectedOrganisations = await TestData.CreateOrganisations(expectedCount);

            var request = new PaginationRequest(0, expectedCount);

            var organisationService = new OrganisationService(dbContext);

            var controller = new OrganisationsController(organisationService);

            // Act
            var result = await controller.GetAllAsync(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var resultOrganisations = okResult
                .Value.Should()
                .BeOfType<PaginationResult<OrganisationDto>>()
                .Subject.Records;

            resultOrganisations.Should().BeEquivalentTo(expectedOrganisations);
        });
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNoContent_WhenNoOrganisationsReturned()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var request = new PaginationRequest(0, 1);

            var organisationService = new OrganisationService(dbContext);

            var controller = new OrganisationsController(organisationService);

            // Act
            var result = await controller.GetAllAsync(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        });
    }

    [Fact]
    public async Task GetOrganisationByIdAsync_ReturnsOkResult_WhenOrganisationExists()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var createdOrganisation = (await TestData.CreateOrganisation("test org")).ToDto();
            var organisationService = new OrganisationService(dbContext);

            var controller = new OrganisationsController(organisationService);

            // Act
            var result = await controller.GetByIdAsync(createdOrganisation.OrganisationId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var resultOrganisations = okResult.Value.Should().BeOfType<OrganisationDto>().Subject;

            resultOrganisations.Should().BeEquivalentTo(createdOrganisation);
        });
    }

    [Fact]
    public async Task GetOrganisationByIdAsync_ReturnsNotFoundResult_WhenUserDoesNotExistWhenOrganisationExists()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var organisationService = new OrganisationService(dbContext);

            var controller = new OrganisationsController(organisationService);

            // Act
            var result = await controller.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NoContentResult>();
        });
    }

     [Fact]
    public async Task CreateAsync_ReturnsCreatedResult_WhenOrganisationIsCreated()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var organisationName = Address.City();
            var organisation = await TestData.CreateOrganisation(organisationName);

            var organisationService = new OrganisationService(dbContext);

            var controller = new OrganisationsController(organisationService);

            // Act
            var result = await controller.CreateAsync(
                new CreateOrganisationRequest
                {
                    OrganisationName = organisationName,
                    ExternalOrganisationId = organisation.ExternalOrganisationId,
                    Type = organisation.Type,
                    LocalAuthorityCode = organisation.LocalAuthorityCode,
                    PrimaryCoordinatorId = organisation.PrimaryCoordinatorId,
                    Region = organisation.Region,
                    CreatePersonRequest = new CreatePersonRequest
                    {
                        FirstName = organisation.PrimaryCoordinator!.FirstName,
                        LastName = organisation.PrimaryCoordinator.LastName,
                        MiddleName = organisation.PrimaryCoordinator.MiddleName,
                        EmailAddress = organisation.PrimaryCoordinator.EmailAddress,
                        PhoneNumber = organisation.PrimaryCoordinator.PhoneNumber,
                        SocialWorkEnglandNumber = organisation.PrimaryCoordinator.Trn,
                        Roles = organisation.PrimaryCoordinator.PersonRoles.Select(x => x.Role.RoleName).ToImmutableList(),
                        Status = organisation.PrimaryCoordinator.Status,
                        ExternalUserId = organisation.PrimaryCoordinator.ExternalUserId,
                        IsFunded = organisation.PrimaryCoordinator.IsFunded,
                        ProgrammeStartDate = organisation.PrimaryCoordinator.ProgrammeStartDate,
                        ProgrammeEndDate = organisation.PrimaryCoordinator.ProgrammeEndDate
                    }
                }
            );

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.Value.Should().BeOfType<OrganisationDto>();
            createdResult
                .Value.Should()
                .BeEquivalentTo(organisation, p => p.Excluding(x => x.OrganisationId));
        });
    }
}
