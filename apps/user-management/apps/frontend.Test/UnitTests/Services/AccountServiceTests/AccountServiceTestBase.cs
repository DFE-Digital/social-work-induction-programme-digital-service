using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Models;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public abstract class AccountServiceTestBase
{
    private protected PersonFaker PersonFaker { get; }

    private protected Mock<IAuthServiceClient> MockClient { get; }

    private protected IModelMapper<Person, Account> Mapper { get; }

    private protected AccountService Sut;

    protected AccountServiceTestBase()
    {
        PersonFaker = new();
        MockClient = new();
        Mapper = new AccountMapper();

        Sut = new(MockClient.Object, Mapper);
    }
}
