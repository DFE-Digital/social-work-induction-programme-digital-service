using System.Collections.Immutable;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using FakeXrmEasy.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers;

public class OrganisationsControllerTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    private readonly AppInfo _appInfo = new();

    [Fact]
    public async Task GetAllAsync_ReturnsOkResult_WhenAccountsExist()
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
            var resultAccounts = okResult
                .Value.Should()
                .BeOfType<PaginationResult<PersonDto>>()
                .Subject.Records;

            resultAccounts.Should().BeEquivalentTo(expectedOrganisations);
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
}
