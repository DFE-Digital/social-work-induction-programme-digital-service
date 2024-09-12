using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.NameMatch.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.SocialWorkEnglandServiceTests;

public abstract class SocialWorkEnglandTestBase
{
    private protected SocialWorkerFaker SocialWorkerFaker;

    private protected Mock<ISocialWorkEnglandClient> MockClient { get; }

    private protected Mock<ISocialWorkerValidatorService> MockSocialWorkerValidatorService { get; }

    private protected SocialWorkEnglandService Sut;

    protected SocialWorkEnglandTestBase()
    {
        SocialWorkerFaker = new();

        MockClient = new();
        MockSocialWorkerValidatorService = new();
        Sut = new(MockClient.Object, MockSocialWorkerValidatorService.Object);
    }
}
