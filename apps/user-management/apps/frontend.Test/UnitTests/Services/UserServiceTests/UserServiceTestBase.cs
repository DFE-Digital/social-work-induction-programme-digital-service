using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.UserServiceTests;

public abstract class UserServiceTestBase
{
    private protected PersonFaker PersonFaker { get; }

    private protected UserBuilder UserBuilder { get; }

    private protected MockAuthServiceClient MockClient { get; }

    private protected IModelMapper<Person, User> Mapper { get; }

    private protected UserService Sut;

    protected UserServiceTestBase()
    {
        PersonFaker = new();
        UserBuilder = new();
        MockClient = new();
        Mapper = new UserMapper();

        Sut = new(MockClient.Object, Mapper);
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockClient.VerifyNoOtherCalls();
    }
}
