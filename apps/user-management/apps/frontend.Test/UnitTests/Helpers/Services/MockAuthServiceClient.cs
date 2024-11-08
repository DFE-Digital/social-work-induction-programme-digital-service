using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Moq;

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

        Setup(x => x.Accounts).Returns(MockAccountsOperations.Object);
    }
}
