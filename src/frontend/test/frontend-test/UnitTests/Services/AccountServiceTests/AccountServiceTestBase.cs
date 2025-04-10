using System.Security.Claims;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Models;
using SocialWorkInductionProgramme.Frontend.Mappers;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Services;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Builders;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Fakers;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Services.AccountServiceTests;

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
