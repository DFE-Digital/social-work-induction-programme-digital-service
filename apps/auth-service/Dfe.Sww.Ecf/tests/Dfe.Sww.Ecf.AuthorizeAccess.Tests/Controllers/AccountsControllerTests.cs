using System.Collections.Immutable;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;
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

            var expectedAccounts = dbContext
                .Persons.Include(p => p.PersonRoles)
                .ThenInclude(pr => pr.Role)
                .Take(expectedCount)
                .Select(p => p.ToDto())
                .ToList();
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
            var resultAccounts = okResult
                .Value.Should()
                .BeOfType<PaginationResult<PersonDto>>()
                .Subject.Records;

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
            var resultAccounts = okResult.Value.Should().BeOfType<PersonDto>().Subject;

            resultAccounts.Should().BeEquivalentTo(createdPerson.ToDto());
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
            var expectedNewUser = (await TestData.CreatePerson(null, false)).ToPersonDto();
            expectedNewUser.Roles = new List<RoleType>
            {
                Faker.Enum.Random<RoleType>(),
            }.ToImmutableList();
            ;
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService);

            // Act
            var result = await controller.CreateAsync(
                new CreatePersonRequest
                {
                    FirstName = expectedNewUser.FirstName,
                    LastName = expectedNewUser.LastName,
                    EmailAddress = expectedNewUser.EmailAddress,
                    SocialWorkEnglandNumber = expectedNewUser.SocialWorkEnglandNumber,
                    Roles = expectedNewUser.Roles,
                }
            );

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.Value.Should().BeOfType<PersonDto>();
            createdResult
                .Value.Should()
                .BeEquivalentTo(expectedNewUser, p => p.Excluding(x => x.PersonId));
        });
    }
}
