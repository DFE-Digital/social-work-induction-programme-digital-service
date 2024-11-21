using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Moq;
using Person = Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Person;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;

public class MockAuthServiceClient : Mock<IAuthServiceClient>
{
    public Mock<IAccountsOperations> MockAccountsOperations { get; }

    public MockAuthServiceClient()
    {
        MockAccountsOperations = new Mock<IAccountsOperations>();
        SetupMockAccountsOperations();
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
                        EmailAddress = createPersonRequest.EmailAddress
                    }
            );

        Setup(x => x.Accounts).Returns(MockAccountsOperations.Object);
    }
}
