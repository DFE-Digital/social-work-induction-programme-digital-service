using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.LocalAuthorities;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers;

[Collection("Uses Database")]
public class LocalAuthorityControllerTests : TestBase
{
    public LocalAuthorityControllerTests(HostFixture hostFixture)
        : base(hostFixture)
    {
        var dbHelper = HostFixture.Services.GetRequiredService<DbHelper>();
        dbHelper.ClearData().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetLocalAuthorityByCodeAsync_ReturnsOkResultWithLocalAuthority_WhenLocalAuthorityExists()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var createdLocalAuthority = (await TestData.CreateLocalAuthority()).ToDto();
            var localAuthorityService = new LocalAuthorityService(dbContext);

            var controller = new LocalAuthorityController(localAuthorityService);

            // Act
            var result = await controller.GetByCodeAsync(createdLocalAuthority.LocalAuthorityCode);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var resultOrganisation = okResult.Value.Should().BeOfType<LocalAuthorityDto>().Subject;

            resultOrganisation.Should().BeEquivalentTo(createdLocalAuthority);
        });
    }

    [Fact]
    public async Task GetOrganisationByLocalAuthorityCodeAsync_ReturnsNotFoundResult_WhenLocalAuthorityDoesNotExist()
    {
        await WithDbContext(async dbContext =>
        {
            // Arrange
            var localAuthorityService = new LocalAuthorityService(dbContext);

            var controller = new LocalAuthorityController(localAuthorityService);

            // Act
            var result = await controller.GetByCodeAsync(999999);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        });
    }
}
