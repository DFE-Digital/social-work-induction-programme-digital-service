using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests;

public class ResetRegisterSocialWorkerJourneyModelShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ResetsUserJourney()
    {
        // Arrange
        var account = AccountBuilder.Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        await Sut.SetDateOfBirthAsync(account.Id, account.DateOfBirth);

        // Act
        Sut.ResetRegisterSocialWorkerJourneyModel(account.Id);

        // Assert
        HttpContext.Session.TryGet(
            RegisterSocialWorkerSessionKey(account.Id),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );

        registerSocialWorkerJourneyModel.Should().BeNull();

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
