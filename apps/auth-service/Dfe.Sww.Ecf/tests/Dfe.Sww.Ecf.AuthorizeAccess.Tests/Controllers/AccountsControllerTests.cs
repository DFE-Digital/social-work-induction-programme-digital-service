using Dfe.Sww.Ecf.AuthorizeAccess.Controllers;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers;

public class AccountsControllerTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    [Fact]
    public async Task GetAllAsync_ReturnsOkResult_WhenAccountsExist()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            const int expectedCount = 5;
            await TestData.CreatePersons(expectedCount);

            var request = new PaginationRequest(0, expectedCount);

            var expectedAccounts = dbContext.Persons.Take(expectedCount).ToList();
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService);

            // Act
            var result = await controller.GetAllAsync(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var resultAccounts = okResult.Value.Should().BeOfType<PaginationResult<Person>>().Subject.Records;

            resultAccounts.Should().BeEquivalentTo(expectedAccounts);
        });
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsOkResult_WhenUserExists()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var createdPerson = (await TestData.CreatePerson()).ToPerson();
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService);

            // Act
            var result = await controller.GetByIdAsync(createdPerson.PersonId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var resultAccounts = okResult.Value.Should().BeOfType<Person>().Subject;

            resultAccounts.Should().BeEquivalentTo(createdPerson);
        });
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNotFoundResult_WhenUserDoesNotExist()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService);

            // Act
            var result = await controller.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NoContentResult>();
        });
    }

    [Fact]
    public async Task GetAccountLinkingTokenAsync_ReturnsOkResult_WhenUserExistsAndLinkingTokenIsGenerated()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var createdPerson = (await TestData.CreatePerson()).ToPerson();
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService);

            // Act
            var result = await controller.GetLinkingTokenByIdAsync(createdPerson.PersonId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeOfType<AccountsController.LinkingTokenResult>();
        });
    }

    [Fact]
    public async Task GetAccountLinkingTokenAsync_ReturnsNotFoundResult_WhenUserDoesNotExist()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService);

            // Act
            var result = await controller.GetLinkingTokenByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NoContentResult>();
        });
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedResult_WhenUserIsCreated()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var expectedNewUser = await TestData.CreatePerson(null, false);
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService);

            // Act
            var result = await controller.CreateAsync(expectedNewUser.ToPerson());

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.Value.Should().BeOfType<Person>();
            createdResult.Value.Should().BeEquivalentTo(expectedNewUser.ToPerson());
        });
    }
}
