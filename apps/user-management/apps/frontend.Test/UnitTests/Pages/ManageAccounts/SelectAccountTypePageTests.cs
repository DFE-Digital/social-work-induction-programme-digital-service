using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SelectAccountTypePageTests : ManageAccountsPageTestBase<SelectAccountType>
{
    private SelectAccountType Sut { get; }

    public SelectAccountTypePageTests()
    {
        Sut = new SelectAccountType(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator(),
            MockEditAccountJourneyService.Object
        );
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Get_WhenCalled_LoadsTheView(bool isStaff)
    {
        // Arrange
        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(isStaff);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts");
        Sut.IsStaff.Should().Be(isStaff);

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetNew_WhenCalled_ResetsJourneyModelAndRedirectsToGetHandler()
    {
        // Arrange
        MockCreateAccountJourneyService.Setup(x => x.ResetCreateAccountJourneyModel());

        // Act
        var result = Sut.OnGetNew();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be("/manage-accounts/select-account-type");

        MockCreateAccountJourneyService.Verify(x => x.ResetCreateAccountJourneyModel(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetEdit_WhenCalled_LoadsTheViewAndPopulatesModel()
    {
        // Arrange
        var account = AccountFaker.Generate();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);

        // Act
        var result = Sut.OnGetEdit(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.EditAccountId.Should().Be(account.Id);
        Sut.IsStaff.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetEdit_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(invalidId)).Returns(false);

        // Act
        var result = Sut.OnGetEdit(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void Post_WhenIsStaffFalse_RedirectsToCorrectPage()
    {
        // Arrange
        Sut.IsStaff = false;

        MockCreateAccountJourneyService.Setup(x => x.SetIsStaff(false));
        MockCreateAccountJourneyService.Setup(x =>
            x.SetAccountTypes(
                MoqHelpers.ShouldBeEquivalentTo(
                    new List<AccountType> { AccountType.EarlyCareerSocialWorker }
                )
            )
        );

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStaff(false), Times.Once);
        MockCreateAccountJourneyService.Verify(
            x =>
                x.SetAccountTypes(
                    MoqHelpers.ShouldBeEquivalentTo(
                        new List<AccountType> { AccountType.EarlyCareerSocialWorker }
                    )
                ),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void Post_WhenIsStaffTrue_RedirectsToSelectUseCase()
    {
        // Arrange
        Sut.IsStaff = true;

        MockCreateAccountJourneyService.Setup(x => x.SetIsStaff(true));

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/select-use-case");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStaff(true), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void Post_WhenModelInvalid_AddsErrorsToModelState()
    {
        // Arrange
        Sut.IsStaff = null;

        // Act
        Sut.ValidateModel();
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<PageResult>();
        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsStaff");
        modelState["IsStaff"]!.Errors.Count.Should().Be(1);
        modelState["IsStaff"]!.Errors[0].ErrorMessage.Should().Be("Select who you want to add");
        Sut.BackLinkPath.Should().Be("/manage-accounts");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void PostEdit_WhenIsStaffFalse_UpdatesAccountTypeAndRedirectsToViewAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        Sut.IsStaff = false;

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);
        MockEditAccountJourneyService.Setup(x => x.SetIsStaff(account.Id, false));
        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountTypes(
                account.Id,
                MoqHelpers.ShouldBeEquivalentTo(
                    new List<AccountType> { AccountType.EarlyCareerSocialWorker }
                )
            )
        );
        MockEditAccountJourneyService.Setup(x => x.CompleteJourney(account.Id));

        // Act
        var result = Sut.OnPostEdit(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.SetIsStaff(account.Id, false), Times.Once);
        MockEditAccountJourneyService.Verify(
            x => x.SetAccountTypes(account.Id, It.IsAny<IEnumerable<AccountType>>()),
            Times.Once
        );
        MockEditAccountJourneyService.Verify(x => x.CompleteJourney(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void PostEdit_WhenIsStaffTrue_RedirectsToEditUseCase()
    {
        // Arrange
        var account = AccountFaker.Generate();
        Sut.IsStaff = true;

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);
        MockEditAccountJourneyService.Setup(x => x.SetIsStaff(account.Id, true));

        // Act
        var result = Sut.OnPostEdit(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts/select-use-case/" + account.Id + "?handler=Edit");

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.SetIsStaff(account.Id, true), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void PostEdit_WhenModelInvalid_AddsErrorsToModelState()
    {
        // Arrange
        var account = AccountFaker.Generate();
        Sut.IsStaff = null;

        // Act
        Sut.ValidateModel();
        var result = Sut.OnPostEdit(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsStaff");
        modelState["IsStaff"]!.Errors.Count.Should().Be(1);
        modelState["IsStaff"]!.Errors[0].ErrorMessage.Should().Be("Select who you want to add");
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void PostEdit_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = Sut.OnPostEdit(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
