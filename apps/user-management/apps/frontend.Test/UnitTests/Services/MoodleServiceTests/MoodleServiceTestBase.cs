using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.MoodleServiceTests;

public abstract class MoodleServiceTestBase
{
    private protected OrganisationBuilder OrganisationBuilder { get; }

    private protected Mock<IMoodleServiceClient> MockClient { get; }

    private protected readonly MoodleService Sut;

    protected MoodleServiceTestBase()
    {
        OrganisationBuilder = new();
        MockClient = new();

        Sut = new(MockClient.Object);
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockClient.VerifyNoOtherCalls();
    }
}
