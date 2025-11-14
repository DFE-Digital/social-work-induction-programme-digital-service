using Moq;
using Xunit;
using FluentAssertions;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.OrganisationServiceTests;

public class ExistsByLocalAuthorityCodeShould : OrganisationAccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsTrueOrFalse()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var laCode = organisation.LocalAuthorityCode ?? 0;

        MockClient.Setup(x => x.Organisations.ExistsByLocalAuthorityCodeAsync(laCode)).ReturnsAsync(true);

        // Act
        var response = await Sut.ExistsByLocalAuthorityCodeAsync(laCode);

        // Assert
        response.Should().BeTrue();

        MockClient.Verify(x => x.Organisations.ExistsByLocalAuthorityCodeAsync(laCode), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
