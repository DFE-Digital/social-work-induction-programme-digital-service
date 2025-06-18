using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class SelectSexAndGenderIdentityPageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectSexAndGenderIdentity Sut { get; }

    public SelectSexAndGenderIdentityPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectSexAndGenderIdentityValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var userSex = UserSex.Female;
        var genderMatchesSexAtBirth = GenderMatchesSexAtBirth.Yes;
        var otherGender = "test value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetUserSexAsync(PersonId)).ReturnsAsync(userSex);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetUserGenderMatchesSexAtBirthAsync(PersonId)).ReturnsAsync(genderMatchesSexAtBirth);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetOtherGenderIdentityAsync(PersonId)).ReturnsAsync(otherGender);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedUserSex.Should().Be(userSex);
        Sut.GenderMatchesSexAtBirth.Should().Be(genderMatchesSexAtBirth);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-date-of-birth");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetUserSexAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetUserGenderMatchesSexAtBirthAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetOtherGenderIdentityAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var otherGender = "test value";
        Sut.SelectedUserSex = UserSex.Male;
        Sut.GenderMatchesSexAtBirth = GenderMatchesSexAtBirth.No;
        Sut.OtherGenderIdentity = otherGender;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-ethnic-group");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetUserSexAsync(PersonId, UserSex.Male), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetUserGenderMatchesSexAtBirthAsync(PersonId, GenderMatchesSexAtBirth.No), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetOtherGenderIdentityAsync(PersonId, otherGender), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedUserSex = null;
        Sut.GenderMatchesSexAtBirth = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(2);

        modelStateKeys.Should().Contain("SelectedUserSex");
        modelState["SelectedUserSex"]!.Errors.Count.Should().Be(1);
        modelState["SelectedUserSex"]!.Errors[0].ErrorMessage.Should()
            .Be("Select your sex");

        modelStateKeys.Should().Contain("GenderMatchesSexAtBirth");
        modelState["GenderMatchesSexAtBirth"]!.Errors.Count.Should().Be(1);
        modelState["GenderMatchesSexAtBirth"]!.Errors[0].ErrorMessage.Should()
            .Be("Select your gender identity");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-date-of-birth");

        VerifyAllNoOtherCalls();
    }
}
