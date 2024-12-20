using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class AddAccountDetailsPageTests : ManageAccountsPageTestBase<AddAccountDetails>
{
    private AddAccountDetails Sut { get; }

    public AddAccountDetailsPageTests()
    {
        Sut = new AddAccountDetails(
            MockCreateAccountJourneyService.Object,
            new AccountDetailsValidator(),
            new FakeLinkGenerator(),
            MockSocialWorkEnglandService.Object
        );
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Get_WhenCalled_LoadsTheView(bool isStaff)
    {
        // Arrange
        Sut.IsStaff = isStaff;
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(isStaff);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        // Assert
        Sut.FirstName.Should().Be(account.FirstName);
        Sut.LastName.Should().Be(account.LastName);
        Sut.Email.Should().Be(account.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(account.SocialWorkEnglandNumber);
        Sut.BackLinkPath.Should()
            .Be(
                isStaff
                    ? "/manage-accounts/select-use-case"
                    : "/manage-accounts/select-account-type"
            );

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData("1")]
    [InlineData(null)]
    public async Task Post_WhenCalledWithoutSocialWorkNumber_RedirectsToConfirmAccountDetails(
        string? sweId
    )
    {
        // Arrange
        var account = AccountBuilder
            .WithSocialWorkEnglandNumber(sweId)
            .WithTypes(ImmutableList.Create(AccountType.EarlyCareerSocialWorker))
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        var socialWorker = sweId is null ? null : new SocialWorker();
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.SocialWorkEnglandNumber = sweId;

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);
        MockSocialWorkEnglandService
            .Setup(x => x.GetByIdAsync(sweId))
            .ReturnsAsync(sweId is null ? null : socialWorker);
        if (socialWorker != null)
        {
            MockCreateAccountJourneyService.Setup(x =>
                x.SetSocialWorkerDetails(MoqHelpers.ShouldBeEquivalentTo(socialWorker))
            );
        }

        MockCreateAccountJourneyService.Setup(x =>
            x.SetAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails))
        );

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be(
                sweId is null
                    ? "/manage-accounts/confirm-account-details"
                    : "/manage-accounts/add-existing-user"
            );

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockSocialWorkEnglandService.Verify(x => x.GetByIdAsync(sweId), Times.Once);
        if (socialWorker != null)
        {
            MockCreateAccountJourneyService.Verify(
                x => x.SetSocialWorkerDetails(MoqHelpers.ShouldBeEquivalentTo(socialWorker)),
                Times.Once
            );
        }

        MockCreateAccountJourneyService.Verify(
            x => x.SetAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToAddAccountDetails()
    {
        // Arrange
        var accountDetails = new AccountDetailsFaker().Generate();
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = string.Empty;

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenSocialWorkerReturnsNull_ReturnsErrors()
    {
        // Arrange
        var sweId = "123";
        var accountDetails = new AccountDetailsFaker().Generate();
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.SocialWorkEnglandNumber = sweId;

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);

        MockSocialWorkEnglandService
            .Setup(x => x.GetByIdAsync(sweId))
            .ReturnsAsync((SocialWorker?)null);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain(nameof(Sut.SocialWorkEnglandNumber));
        modelState[nameof(Sut.SocialWorkEnglandNumber)]!.Errors.Count.Should().Be(1);
        modelState[nameof(Sut.SocialWorkEnglandNumber)]!
            .Errors[0]
            .ErrorMessage.Should()
            .Be("Failed to retrieve Social Work England record. Please try again later.");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockSocialWorkEnglandService.Verify(x => x.GetByIdAsync(sweId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Post_WhenCalledWithInvalidData_HasCorrectBackLink(bool isStaff)
    {
        // Arrange
        Sut.IsStaff = isStaff;

        // Act
        _ = await Sut.OnPostAsync();

        // Assert
        Sut.BackLinkPath.Should()
            .Be(
                isStaff
                    ? "/manage-accounts/select-use-case"
                    : "/manage-accounts/select-account-type"
            );
    }

    [Fact]
    public async Task PostChange_WhenCalled_HasCorrectBackLink()
    {
        // Act
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
    }
}
