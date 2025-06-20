using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests;

public class SetOtherRouteIntoSocialWorkShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsOtherRouteIntoSocialWork()
    {
        // Arrange
        var originalAccount = AccountBuilder.Build();

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.SetOtherRouteIntoSocialWorkAsync(originalAccount.Id, originalAccount.OtherRouteIntoSocialWork);

        // Assert
        HttpContext.Session.TryGet(
            RegisterSocialWorkerSessionKey(originalAccount.Id),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );

        registerSocialWorkerJourneyModel.Should().NotBeNull();
        registerSocialWorkerJourneyModel!.OtherRouteIntoSocialWork.Should().Be(originalAccount.OtherRouteIntoSocialWork);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
