using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class ResetEditUserJourneyModelShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ResetsUserJourney()
    {
        // Arrange
        var user = UserBuilder.Build();

        MockUserService.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        await Sut.SetIsStaffAsync(user.Id, user.IsStaff);

        // Act
        await Sut.ResetEditUserJourneyModelAsync(user.Id);

        // Assert
        HttpContext.Session.TryGet(
            EditUserSessionKey(user.Id),
            out EditUserJourneyModel? editUserJourneyModel
        );

        editUserJourneyModel.Should().BeNull();

        MockUserService.Verify(x => x.GetByIdAsync(user.Id), Times.Exactly(2));
        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenUserNotFound_ThrowExpectedException()
    {
        // Arrange
        var user = UserBuilder.Build();

        var expectedException = new KeyNotFoundException("User not found with ID " + user.Id);

        MockUserService.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync((User?)null);

        // Act
        var actualException = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => Sut.ResetEditUserJourneyModelAsync(user.Id)
        );

        // Assert
        actualException.Message.Should().Be(expectedException.Message);

        MockUserService.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
