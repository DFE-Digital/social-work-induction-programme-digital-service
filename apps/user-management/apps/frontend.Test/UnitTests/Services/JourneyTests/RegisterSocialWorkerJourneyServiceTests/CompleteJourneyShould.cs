using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests;

public class CompleteJourneyShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CompletesJourney()
    {
        // Arrange
        var account = AccountBuilder
            .WithSocialWorkEnglandNumber()
            .WithStatus(AccountStatus.Active)
            .Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);
        MockAccountService.Setup(x => x.UpdateAsync(account));
        WelcomeEmailRequest? capturedEmailRequest = null;
        MockEmailService.Setup(x => x.SendWelcomeEmailAsync(It.IsAny<WelcomeEmailRequest>()))
            .Callback<WelcomeEmailRequest>(req => capturedEmailRequest = req);

        // Act
        await Sut.CompleteJourneyAsync(account.Id);

        // Assert
        HttpContext.Session.TryGet(
            RegisterSocialWorkerSessionKey(account.Id),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );

        registerSocialWorkerJourneyModel.Should().BeNull();

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        MockAccountService.Verify(
            x => x.UpdateAsync(MoqHelpers.ShouldBeEquivalentTo(account)),
            Times.Once
        );
        MockEmailService.Verify(x =>
            x.SendWelcomeEmailAsync(It.IsAny<WelcomeEmailRequest>()), Times.Once);
        capturedEmailRequest.Should().BeEquivalentTo(new WelcomeEmailRequest
        {
            AccountId = account.Id
        });
        VerifyAllNoOtherCall();
    }
}
