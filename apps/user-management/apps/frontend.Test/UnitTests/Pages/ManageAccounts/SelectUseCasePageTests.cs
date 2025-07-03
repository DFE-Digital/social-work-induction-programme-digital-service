using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SelectUseCasePageTests : ManageAccountsPageTestBase<SelectUseCase>
{
    private SelectUseCase Sut { get; }

    public SelectUseCasePageTests()
    {
        Sut = new SelectUseCase(
            MockCreateAccountJourneyService.Object,
            new SelectUseCaseValidator(),
            new FakeLinkGenerator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.SelectedAccountTypes.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-accounts/select-account-type");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithNullSelectedAccountTypes_ReturnsErrorsAndRedirectsToSelectAccountType()
    {
        // Arrange
        Sut.SelectedAccountTypes = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("SelectedAccountTypes");
        modelState["SelectedAccountTypes"]!.Errors.Count.Should().Be(1);
        modelState["SelectedAccountTypes"]!.Errors[0].ErrorMessage.Should().Be("Select what the user needs to do");

        Sut.BackLinkPath.Should().Be("/manage-accounts/select-account-type");

        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true, "/manage-accounts/confirm-account-details")]
    [InlineData(false, "/manage-accounts/add-account-details")]
    public async Task OnPostAsync_WhenSelectedAccountTypesIsPopulated_RedirectsToRelevantPageBasedOnFromChangeLink(
        bool fromChangeLink,
        string redirectPath
    )
    {
        // Arrange
        Sut.SelectedAccountTypes = new List<AccountType> { AccountType.Assessor };
        Sut.FromChangeLink = fromChangeLink;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(redirectPath);

        MockCreateAccountJourneyService.Verify(x => x.SetAccountTypes(It.IsAny<List<AccountType>>()), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalled_HasFromChangeLinkTrue()
    {
        // Act
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.FromChangeLink.Should().BeTrue();
    }
}
