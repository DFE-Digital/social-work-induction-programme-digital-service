using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class CompleteJourneyShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CompletesJourney()
    {
        // Arrange
        var account = AccountBuilder.WithId(Guid.Empty).Build();

        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel
            {
                AccountDetails = AccountDetails.FromAccount(account),
                AccountTypes = account.Types,
                IsStaff = account.IsStaff
            }
        );

        var expected = account with { Id = Guid.NewGuid() };
        MockAccountService.Setup(x => x.CreateAsync(It.IsAny<Account>(), It.IsAny<Guid?>())).ReturnsAsync(expected);
        InvitationEmailRequest? capturedEmailRequest = null;
        MockEmailService.Setup(x => x.SendInvitationEmailAsync(It.IsAny<InvitationEmailRequest>())).Callback<InvitationEmailRequest>(req => capturedEmailRequest = req);

        // Act
        var result = await Sut.CompleteJourneyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Account>();
        result.Should().BeEquivalentTo(expected);
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );
        createAccountJourneyModel.Should().BeNull();

        MockAccountService.Verify(
            x =>
                x.CreateAsync(
                    It.Is<Account>(acc =>
                        acc.Id == account.Id
                        && acc.FullName == account.FullName
                        && acc.Email == account.Email
                        && acc.IsStaff == account.IsStaff
                        && acc.Types != null
                        && acc.Types.SequenceEqual(account.Types!)
                        && acc.SocialWorkEnglandNumber == account.SocialWorkEnglandNumber
                    ),
                    It.IsAny<Guid?>()
                ),
            Times.Once
        );
        MockEmailService.Verify(x =>
            x.SendInvitationEmailAsync(It.IsAny<InvitationEmailRequest>()), Times.Once);
        capturedEmailRequest.Should().BeEquivalentTo(new InvitationEmailRequest
        {
            AccountId = expected.Id,
            OrganisationName = "Test Organisation",
            Role = expected.Types!.Min()
        });

        VerifyAllNoOtherCall();
    }
}
