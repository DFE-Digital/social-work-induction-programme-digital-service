using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class SetUserTypesShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsUserTypes()
    {
        // Arrange
        var originalUser = UserBuilder.Build();

        var updatedUser = UserBuilder.Build();
        var editedUserTypes = new EditUserJourneyModel(updatedUser).UserTypes!;

        MockUserService
            .Setup(x => x.GetByIdAsync(originalUser.Id))
            .ReturnsAsync(originalUser);

        // Act
        await Sut.SetUserTypesAsync(originalUser.Id, editedUserTypes);

        // Assert
        HttpContext.Session.TryGet(
            EditUserSessionKey(originalUser.Id),
            out EditUserJourneyModel? editUserJourneyModel
        );

        editUserJourneyModel.Should().NotBeNull();
        editUserJourneyModel!.UserTypes.Should().BeEquivalentTo(editedUserTypes);

        MockUserService.Verify(x => x.GetByIdAsync(originalUser.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
