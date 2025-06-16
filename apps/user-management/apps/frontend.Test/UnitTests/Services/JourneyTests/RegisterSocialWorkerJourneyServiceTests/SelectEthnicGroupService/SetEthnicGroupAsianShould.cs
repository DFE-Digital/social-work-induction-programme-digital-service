using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests.SelectEthnicGroupService;

public class SetEthnicGroupAsianShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsEthnicGroupAsian()
    {
        // Arrange
        var originalAccount = AccountBuilder.Build();

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.EthnicGroupService.SetEthnicGroupAsianAsync(originalAccount.Id, originalAccount.EthnicGroupAsian);

        // Assert
        HttpContext.Session.TryGet(
            RegisterSocialWorkerSessionKey(originalAccount.Id),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );

        registerSocialWorkerJourneyModel.Should().NotBeNull();
        registerSocialWorkerJourneyModel!.EthnicGroupAsian.Should().Be(originalAccount.EthnicGroupAsian);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
