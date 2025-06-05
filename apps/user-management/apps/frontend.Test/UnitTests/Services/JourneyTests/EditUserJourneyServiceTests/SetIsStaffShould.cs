using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class SetIsStaffShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsIsStaff()
    {
        // Arrange
        var originalUser = UserBuilder.Build();

        MockUserService
            .Setup(x => x.GetByIdAsync(originalUser.Id))
            .ReturnsAsync(originalUser);

        // Act
        await Sut.SetIsStaffAsync(originalUser.Id, !originalUser.IsStaff);

        // Assert
        HttpContext.Session.TryGet(
            EditUserSessionKey(originalUser.Id),
            out EditUserJourneyModel? editUserJourneyModel
        );

        editUserJourneyModel.Should().NotBeNull();
        editUserJourneyModel!.IsStaff.Should().Be(!originalUser.IsStaff);

        MockUserService.Verify(x => x.GetByIdAsync(originalUser.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
