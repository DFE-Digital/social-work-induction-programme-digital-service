using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public abstract class AccountServiceTestBase
{
    private protected PersonFaker PersonFaker { get; }

    private protected AccountBuilder AccountBuilder { get; }

    private protected MockAuthServiceClient MockClient { get; }

    private protected IModelMapper<Person, Account> Mapper { get; }

    private protected AccountService Sut;

    protected AccountServiceTestBase()
    {
        PersonFaker = new();
        AccountBuilder = new();
        MockClient = new();
        Mapper = new AccountMapper();

        Sut = new(MockClient.Object, Mapper);
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockClient.VerifyNoOtherCalls();
    }
}
