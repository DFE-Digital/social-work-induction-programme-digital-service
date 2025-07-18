using System.Collections.Immutable;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using FakeXrmEasy.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers;

public class AccountsControllerTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    private readonly AppInfo _appInfo = new();

    [Fact]
    public async Task GetAllAsync_ReturnsOkResult_WhenAccountsExist()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            const int expectedCount = 5;
            var organisationName = Faker.Address.City();
            var organisation = await TestData.CreateOrganisation(organisationName);

            await TestData.CreatePersons(expectedCount, organisation.OrganisationId);

            var request = new PaginationRequest(0, expectedCount);

            var filteredAccounts = dbContext.Persons
                .Include(p => p.PersonOrganisations)
                .Where(p => p.PersonOrganisations.Any(o => o.OrganisationId == organisation.OrganisationId))
                .Select(p => p);

            var expectedAccounts = filteredAccounts
                .Include(p => p.PersonRoles)
                .ThenInclude(pr => pr.Role)
                .Take(expectedCount)
                .Select(p => p.ToDto())
                .ToList();
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

            // Act
            var result = await controller.GetAllAsync(request, organisation.OrganisationId.ToString());

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
    public async Task GetAllAsync_ReturnsBadRequest_WhenOrganisationIdIsInvalid()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var organisationId = "invalid-guid";
            var request = new PaginationRequest(0, 1);

            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

            // Act
            var result = await controller.GetAllAsync(request, organisationId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should()
                .Be("Invalid Organisation ID format. Must be a valid GUID.");
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

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

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

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

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

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

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

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

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
            var organisationName = Faker.Address.City();
            var organisation = await TestData.CreateOrganisation(organisationName);
            var expectedNewUser =
                (await TestData
                    .CreatePerson(
                        b => b.WithOrganisationId(organisation.OrganisationId),
                        false
                    )
                ).ToPersonDto();
            expectedNewUser.Roles = new List<RoleType>
            {
                Faker.Enum.Random<RoleType>(),
            }.ToImmutableList();

            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

            // Act
            var result = await controller.CreateAsync(
                new CreatePersonRequest
                {
                    FirstName = expectedNewUser.FirstName,
                    MiddleName = expectedNewUser.MiddleName,
                    LastName = expectedNewUser.LastName,
                    EmailAddress = expectedNewUser.EmailAddress,
                    PhoneNumber = expectedNewUser.PhoneNumber,
                    SocialWorkEnglandNumber = expectedNewUser.SocialWorkEnglandNumber,
                    Roles = expectedNewUser.Roles,
                    Status = expectedNewUser.Status,
                    OrganisationId = organisation.OrganisationId,
                    IsFunded = expectedNewUser.IsFunded,
                    ProgrammeStartDate = expectedNewUser.ProgrammeStartDate,
                    ProgrammeEndDate = expectedNewUser.ProgrammeEndDate
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

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedResult_WhenUserIsUpdated()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var existingUser = (
                await TestData.CreatePerson(p => p.WithStatus(PersonStatus.Active))
            ).ToPersonDto();
            existingUser.Roles = new List<RoleType>
            {
                Faker.Enum.Random<RoleType>(),
            }.ToImmutableList();

            var expectedUser = new PersonDto
            {
                PersonId = existingUser.PersonId,
                CreatedOn = existingUser.CreatedOn,
                UpdatedOn = Clock.UtcNow,
                FirstName = "Changed First Name",
                MiddleName = existingUser.MiddleName,
                LastName = "Changed Last Name",
                EmailAddress = "Changed Email",
                SocialWorkEnglandNumber = "123",
                Roles = new List<RoleType>
                {
                    RoleType.Assessor,
                    RoleType.Coordinator,
                    RoleType.EarlyCareerSocialWorker,
                }.ToImmutableList(),
                Status = PersonStatus.Inactive,
                IsFunded = existingUser.IsFunded,
                ProgrammeStartDate = existingUser.ProgrammeStartDate,
                ProgrammeEndDate = existingUser.ProgrammeEndDate,
                DateOfBirth = existingUser.DateOfBirth,
                UserSex = existingUser.UserSex,
                GenderMatchesSexAtBirth = existingUser.GenderMatchesSexAtBirth,
                OtherGenderIdentity = existingUser.OtherRouteIntoSocialWork,
                EthnicGroup = existingUser.EthnicGroup,
                EthnicGroupWhite = existingUser.EthnicGroupWhite,
                OtherEthnicGroupWhite = existingUser.OtherEthnicGroupWhite,
                EthnicGroupAsian = existingUser.EthnicGroupAsian,
                OtherEthnicGroupAsian = existingUser.OtherEthnicGroupAsian,
                EthnicGroupMixed = existingUser.EthnicGroupMixed,
                OtherEthnicGroupMixed = existingUser.OtherEthnicGroupMixed,
                EthnicGroupBlack = existingUser.EthnicGroupBlack,
                OtherEthnicGroupBlack = existingUser.OtherEthnicGroupBlack,
                EthnicGroupOther = existingUser.EthnicGroupOther,
                OtherEthnicGroupOther = existingUser.OtherEthnicGroupOther,
                Disability = existingUser.Disability,
                SocialWorkEnglandRegistrationDate = existingUser.SocialWorkEnglandRegistrationDate,
                HighestQualification = existingUser.HighestQualification,
                SocialWorkQualificationEndYear = existingUser.SocialWorkQualificationEndYear,
                RouteIntoSocialWork = existingUser.RouteIntoSocialWork,
                OtherRouteIntoSocialWork = existingUser.OtherRouteIntoSocialWork
            };

            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

            // Act
            var result = await controller.UpdateAsync(
                new UpdatePersonRequest
                {
                    PersonId = existingUser.PersonId,
                    FirstName = expectedUser.FirstName,
                    LastName = expectedUser.LastName,
                    EmailAddress = expectedUser.EmailAddress,
                    SocialWorkEnglandNumber = expectedUser.SocialWorkEnglandNumber,
                    Roles = expectedUser.Roles,
                    Status = expectedUser.Status,
                    DateOfBirth = existingUser.DateOfBirth,
                    UserSex = existingUser.UserSex,
                    GenderMatchesSexAtBirth = existingUser.GenderMatchesSexAtBirth,
                    OtherGenderIdentity = existingUser.OtherRouteIntoSocialWork,
                    EthnicGroup = existingUser.EthnicGroup,
                    EthnicGroupWhite = existingUser.EthnicGroupWhite,
                    OtherEthnicGroupWhite = existingUser.OtherEthnicGroupWhite,
                    EthnicGroupAsian = existingUser.EthnicGroupAsian,
                    OtherEthnicGroupAsian = existingUser.OtherEthnicGroupAsian,
                    EthnicGroupMixed = existingUser.EthnicGroupMixed,
                    OtherEthnicGroupMixed = existingUser.OtherEthnicGroupMixed,
                    EthnicGroupBlack = existingUser.EthnicGroupBlack,
                    OtherEthnicGroupBlack = existingUser.OtherEthnicGroupBlack,
                    EthnicGroupOther = existingUser.EthnicGroupOther,
                    OtherEthnicGroupOther = existingUser.OtherEthnicGroupOther,
                    Disability = existingUser.Disability,
                    SocialWorkEnglandRegistrationDate = existingUser.SocialWorkEnglandRegistrationDate,
                    HighestQualification = existingUser.HighestQualification,
                    SocialWorkQualificationEndYear = existingUser.SocialWorkQualificationEndYear,
                    RouteIntoSocialWork = existingUser.RouteIntoSocialWork,
                    OtherRouteIntoSocialWork = existingUser.OtherRouteIntoSocialWork
                }
            );

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var updatedResult = result.Should().BeOfType<OkObjectResult>().Subject;
            updatedResult.Value.Should().BeOfType<PersonDto>();
            updatedResult.Value.Should().BeEquivalentTo(expectedUser);
        });
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenUserIsDeleted()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var existingUser = (
                await TestData.CreatePerson(p => p.WithStatus(PersonStatus.Active))
            ).ToPersonDto();
            existingUser.Roles = new List<RoleType>
            {
                Faker.Enum.Random<RoleType>(),
            }.ToImmutableList();

            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

            // Act
            var result = await controller.DeleteAsync(existingUser.PersonId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        });
    }

    [Fact]
    public async Task DeleteAsync_ReturnsBadRequest_WhenUserIsNotFound()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var accountsService = new AccountsService(dbContext, Clock);
            var oneLoginAccountLinkingService = new OneLoginAccountLinkingService(
                accountsService,
                new MemoryCache(new MemoryCacheOptions())
            );

            var controller = new AccountsController(accountsService, oneLoginAccountLinkingService, _appInfo);

            // Act
            var result = await controller.DeleteAsync(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var response = result as BadRequestObjectResult;
            response!.Value.Should().Be("Account not found.");
        });
    }
}
