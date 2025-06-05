using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class SetUserDetailsShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsUserDetails()
    {
        // Arrange
        var originalUser = UserBuilder.Build();

        var updatedUser = UserBuilder.Build();
        var updatedUserDetails = UserDetails.FromUser(updatedUser);
        var expected = new EditUserJourneyModel(updatedUser).UserDetails;

        MockUserService
            .Setup(x => x.GetByIdAsync(originalUser.Id))
            .ReturnsAsync(originalUser);

        // Act
        await Sut.SetUserDetailsAsync(originalUser.Id, updatedUserDetails);

        // Assert
        HttpContext.Session.TryGet(
            EditUserSessionKey(originalUser.Id),
            out EditUserJourneyModel? editUserJourneyModel
        );

        editUserJourneyModel.Should().NotBeNull();
        editUserJourneyModel!.UserDetails.Should().BeEquivalentTo(expected);

        MockUserService.Verify(x => x.GetByIdAsync(originalUser.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
