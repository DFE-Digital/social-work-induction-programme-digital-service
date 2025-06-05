using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class SetUserStatusShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsUserStatus()
    {
        // Arrange
        var originalUser = UserBuilder.Build();

        var updatedUser = UserBuilder.Build();
        var editedUserStatus = new EditUserJourneyModel(updatedUser).UserStatus!;

        MockUserService
            .Setup(x => x.GetByIdAsync(originalUser.Id))
            .ReturnsAsync(originalUser);

        // Act
        await Sut.SetUserStatusAsync(originalUser.Id, editedUserStatus.Value);

        // Assert
        HttpContext.Session.TryGet(
            EditUserSessionKey(originalUser.Id),
            out EditUserJourneyModel? editUserJourneyModel
        );

        editUserJourneyModel.Should().NotBeNull();
        editUserJourneyModel!.UserStatus.Should().Be(editedUserStatus.Value);

        MockUserService.Verify(x => x.GetByIdAsync(originalUser.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
