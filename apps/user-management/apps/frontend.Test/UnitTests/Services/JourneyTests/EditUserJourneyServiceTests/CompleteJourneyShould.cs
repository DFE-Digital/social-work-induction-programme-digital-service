using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class CompleteJourneyShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CompletesJourney()
    {
        // Arrange
        var user = UserBuilder
            .WithSocialWorkEnglandNumber()
            .WithStatus(UserStatus.Active)
            .Build();

        MockUserService.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
        MockUserService.Setup(x => x.UpdateAsync(user));

        // Act
        await Sut.CompleteJourneyAsync(user.Id);

        // Assert
        HttpContext.Session.TryGet(
            EditUserSessionKey(user.Id),
            out EditUserJourneyModel? editUserJourneyModel
        );

        editUserJourneyModel.Should().BeNull();

        MockUserService.Verify(x => x.GetByIdAsync(user.Id), Times.Exactly(2));
        MockUserService.Verify(
            x => x.UpdateAsync(MoqHelpers.ShouldBeEquivalentTo(user)),
            Times.Once
        );
        VerifyAllNoOtherCall();
    }
}
