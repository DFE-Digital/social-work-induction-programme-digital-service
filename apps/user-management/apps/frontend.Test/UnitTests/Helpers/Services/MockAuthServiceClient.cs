using System.Security.Claims;
using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Person = Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Person;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;

public class MockAuthServiceClient : Mock<IAuthServiceClient>
{
    public Mock<IAccountsOperations> MockAccountsOperations { get; }

    private Mock<IHttpContextAccessor> MockHttpContextAccessor { get; }

    public Mock<IHttpContextService> MockHttpContextService { get; }

    public MockAuthServiceClient()
    {
        MockAccountsOperations = new Mock<IAccountsOperations>();
        MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        MockHttpContextService = new Mock<IHttpContextService>();

        SetupMockAccountsOperations();
        SetupMockHttpContextService();
    }

    private void SetupMockAccountsOperations()
    {
        MockAccountsOperations
            .Setup(operations => operations.GetLinkingTokenByAccountIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Faker().Random.String(64));
        MockAccountsOperations
            .Setup(operations => operations.CreateAsync(It.IsAny<CreatePersonRequest>()))
            .ReturnsAsync(
                (CreatePersonRequest createPersonRequest) =>
                    new Person
                    {
                        PersonId = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        FirstName = createPersonRequest.FirstName,
                        LastName = createPersonRequest.LastName,
                        EmailAddress = createPersonRequest.EmailAddress,
                        SocialWorkEnglandNumber = createPersonRequest.SocialWorkEnglandNumber,
                        Roles = createPersonRequest.Roles
                    }
            );
        MockAccountsOperations
            .Setup(operations => operations.UpdateAsync(It.IsAny<UpdatePersonRequest>()))
            .ReturnsAsync(
                (UpdatePersonRequest updatePersonRequest) =>
                    new Person
                    {
                        PersonId = Guid.NewGuid(),
                        CreatedOn = DateTime.UtcNow,
                        FirstName = updatePersonRequest.FirstName,
                        LastName = updatePersonRequest.LastName,
                        EmailAddress = updatePersonRequest.EmailAddress,
                        SocialWorkEnglandNumber = updatePersonRequest.SocialWorkEnglandNumber,
                        Roles = updatePersonRequest.Roles
                    }
            );
        Setup(x => x.Accounts).Returns(MockAccountsOperations.Object);
    }

    private void SetupMockHttpContextService()
    {
        MockHttpContextService.Setup(a => a.GetOrganisationId()).Returns(Guid.NewGuid().ToString);

        Setup(x => x.HttpContextService).Returns(MockHttpContextService.Object);
    }

    public void SetupMockHttpContextAccessorWithOrganisationId()
    {
        var claims = new List<Claim> { new Claim("organisation_id", Guid.NewGuid().ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var mockHttpContext = new DefaultHttpContext { User = claimsPrincipal };
        MockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext);
    }

    public void SetupMockHttpContextAccessorWithPersonId(Guid? personId = null)
    {
        var claims = new List<Claim> { new Claim("person_id", personId?.ToString() ?? Guid.NewGuid().ToString() ) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var mockHttpContext = new DefaultHttpContext { User = claimsPrincipal };
        MockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext);
    }

    public void SetupMockHttpContextAccessorWithEmptyClaimsPrincipal()
    {
        var emptyClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var mockHttpContext = new DefaultHttpContext { User = emptyClaimsPrincipal };

        MockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext);
    }
}
