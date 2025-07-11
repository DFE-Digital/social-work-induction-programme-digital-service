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
            MockEditAccountJourneyService.Object,
            new SelectUseCaseValidator(),
            new FakeLinkGenerator()
        );
    }

    [Fact]
    public async Task OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.SelectedAccountTypes.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-accounts/select-account-type");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGet_WhenCalledWithId_LoadsTheView()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(id)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.SelectedAccountTypes.Should().BeEquivalentTo(accountDetails.Types);
        Sut.BackLinkPath.Should().Be($"/manage-accounts/view-account-details/{id}");
        Sut.Id.Should().Be(id);

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
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

    [Fact]
    public async Task OnPostAsync_WhenValidDataAndNotFromChangeLink_RedirectsToAddAccountDetails()
    {
        // Arrange
        Sut.SelectedAccountTypes = new List<AccountType> { AccountType.Assessor };
        Sut.FromChangeLink = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetAccountTypes(It.IsAny<List<AccountType>>()), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenFromChangeLinkAndChangingFromAssessorToCoordinator_ClearSocialEnglandNumber()
    {
        // Arrange
        Sut.SelectedAccountTypes = new List<AccountType> { AccountType.Coordinator };
        Sut.FromChangeLink = true;
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithTypes([AccountType.Assessor])
            .WithSocialWorkEnglandNumber("12433")
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/confirm-account-details");
        MockCreateAccountJourneyService.Verify(x =>
                x.SetAccountDetails(
                    It.Is<AccountDetails>(a =>
                        a.FirstName == accountDetails.FirstName
                        && a.LastName == accountDetails.LastName
                        && a.Email == accountDetails.Email
                        && a.SocialWorkEnglandNumber == null
                        && a.IsStaff == accountDetails.IsStaff
                        && a.Types == accountDetails.Types
                    )
                ),
            Times.Once
        );
        MockCreateAccountJourneyService.Verify(x => x.SetAccountTypes(It.IsAny<List<AccountType>>()), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenFromChangeLinkAndSelectedAccountTypeIncludesAssessor_FlagSocialEnglandNumberForCapture()
    {
        // Arrange
        Sut.SelectedAccountTypes = new List<AccountType> { AccountType.Assessor };
        Sut.FromChangeLink = true;
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithTypes([AccountType.Coordinator])
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");
        MockCreateAccountJourneyService.Verify(x => x.SetAccountTypes(It.IsAny<List<AccountType>>()), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithId_RedirectsToPage()
    {
        // Arrange
        var id = Guid.NewGuid();

        Sut.Id = id;
        Sut.SelectedAccountTypes = new List<AccountType> { AccountType.Assessor };

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-accounts/confirm-account-details/{id}?handler=Update");

        MockEditAccountJourneyService.Verify(x => x.SetAccountTypesAsync(id, MoqHelpers.ShouldBeEquivalentTo(Sut.SelectedAccountTypes)), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIdFromChangeSwitchFromCoordinatorToAssessor_RedirectsToEditAccountDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithTypes([AccountType.Coordinator])
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(id)).ReturnsAsync(accountDetails);

        Sut.FromChangeLink = true;
        Sut.Id = id;
        Sut.SelectedAccountTypes = new List<AccountType> { AccountType.Assessor };

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-accounts/edit-account-details/{id}");

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.SetAccountTypesAsync(id, MoqHelpers.ShouldBeEquivalentTo(Sut.SelectedAccountTypes)), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIdFromChangeSwitchFromAssessorToCoordinator_RedirectsToConfirmDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithTypes([AccountType.Assessor])
            .WithSocialWorkEnglandNumber()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(id)).ReturnsAsync(accountDetails);


        Sut.FromChangeLink = true;
        Sut.Id = id;
        Sut.SelectedAccountTypes = new List<AccountType> { AccountType.Coordinator };

        var updatedAccountDetails = accountDetails;
        updatedAccountDetails.SocialWorkEnglandNumber = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-accounts/confirm-account-details/{id}?handler=Update");

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.SetAccountTypesAsync(id, MoqHelpers.ShouldBeEquivalentTo(Sut.SelectedAccountTypes)), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.SetAccountDetailsAsync(id, MoqHelpers.ShouldBeEquivalentTo(updatedAccountDetails)), Times.Once);

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
