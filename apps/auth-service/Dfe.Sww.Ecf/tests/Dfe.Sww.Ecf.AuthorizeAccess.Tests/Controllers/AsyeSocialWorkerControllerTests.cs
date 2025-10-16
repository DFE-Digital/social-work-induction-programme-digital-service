using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.AsyeSocialWorkers;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers;

[Collection("Uses Database")]
public class AsyeSocialWorkerControllerTests : TestBase
{
    private readonly AppInfo _appInfo = new();

    public AsyeSocialWorkerControllerTests(HostFixture hostFixture) : base(hostFixture)
    {
        var dbHelper = HostFixture.Services.GetRequiredService<DbHelper>();
        dbHelper.ClearData().GetAwaiter().GetResult();
    }

    [Theory]
    [InlineData("SW8378", true)]
    [InlineData("sw8378", true)]
    [InlineData("SW2793", true)]
    [InlineData("sw2793", true)]
    [InlineData("1234", false)]
    [InlineData("SW1234", false)]
    public void Exists_ReturnsOkResult_WhenAsyeSocialWorkerIdExists(string socialWorkerId, bool expectedResult)
    {
        // Arrange
        var asyeSocialWorkerService = new AsyeSocialWorkerService();

        var controller = new AsyeSocialWorkerController(asyeSocialWorkerService);

        // Act
        var result = controller.Exists(socialWorkerId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var resultAccounts = okResult.Value.Should().BeOfType<bool>().Subject;

        resultAccounts.Should().Be(expectedResult);
    }
}
