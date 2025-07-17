using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public class CheckYourAnswersPageTests : ManageOrganisationsPageTestBase<CheckYourAnswers>
{
    private ConfirmOrganisationDetails Sut { get; }

    public CheckYourAnswersPageTests()
    {
        Sut = new ConfirmOrganisationDetails(
            MockCreateOrganisationJourneyService.Object,
            new FakeLinkGenerator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.Organisation.Should().BeEquivalentTo(organisation);
        Sut.BackLinkPath.Should().Be("/manage-organisations/enter-local-authority-code");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalled_RedirectsUser()
    {
        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations"); // TODO update this to primary coordinator

        VerifyAllNoOtherCalls();
    }
}
