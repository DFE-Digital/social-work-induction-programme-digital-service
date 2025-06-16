using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests.SelectEthnicGroupService;

public class SetEthnicGroupOtherShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsEthnicGroupOther()
    {
        // Arrange
        var originalAccount = AccountBuilder.Build();

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.EthnicGroupService.SetEthnicGroupOtherAsync(originalAccount.Id, originalAccount.EthnicGroupOther);

        // Assert
        HttpContext.Session.TryGet(
            RegisterSocialWorkerSessionKey(originalAccount.Id),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );

        registerSocialWorkerJourneyModel.Should().NotBeNull();
        registerSocialWorkerJourneyModel!.EthnicGroupOther.Should().Be(originalAccount.EthnicGroupOther);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
