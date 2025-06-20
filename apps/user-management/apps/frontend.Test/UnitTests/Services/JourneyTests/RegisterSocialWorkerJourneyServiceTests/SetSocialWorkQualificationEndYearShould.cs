using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests;

public class SetSocialWorkQualificationEndYearShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsSocialWorkEnglandQualificationEndYear()
    {
        // Arrange
        var originalAccount = AccountBuilder.Build();

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.SetSocialWorkQualificationEndYearAsync(originalAccount.Id, originalAccount.SocialWorkQualificationEndYear);

        // Assert
        HttpContext.Session.TryGet(
            RegisterSocialWorkerSessionKey(originalAccount.Id),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );

        registerSocialWorkerJourneyModel.Should().NotBeNull();
        registerSocialWorkerJourneyModel!.SocialWorkEnglandQualificationEndYear.Should().Be(originalAccount.SocialWorkQualificationEndYear);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
