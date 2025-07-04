using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class CheckYourAnswersPageTests : SocialWorkerRegistrationPageTestBase
{
    private CheckYourAnswers Sut { get; }

    public CheckYourAnswersPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object);
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var journeyModel = new RegisterSocialWorkerJourneyModel(account);
        var changeLinks = GetEscwRegisterChangeLinksFaker.Generate();

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetRegisterSocialWorkerJourneyModelAsync(PersonId)).ReturnsAsync(journeyModel);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetEscwRegisterChangeLinks(account.EthnicGroup)).Returns(changeLinks);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.DateOfBirth.Should().Be(journeyModel.DateOfBirth?.ToString("d MMMM yyyy"));
        Sut.UserSex.Should().Be(journeyModel.UserSex);
        Sut.GenderMatchesSexAtBirth.Should().Be(journeyModel.GenderMatchesSexAtBirth);
        Sut.OtherGenderIdentity.Should().Be(journeyModel.OtherGenderIdentity);
        Sut.EthnicGroup.Should().Be(journeyModel.EthnicGroup);
        Sut.OtherEthnicGroup.Should().Be(GetOtherEthnicGroup(journeyModel));
        Sut.Disability.Should().Be(journeyModel.Disability);
        Sut.SocialWorkEnglandRegistrationDate.Should().Be(journeyModel.SocialWorkEnglandRegistrationDate?.ToString("d MMMM yyyy"));
        Sut.SocialWorkQualificationEndYear.Should().Be(journeyModel.SocialWorkQualificationEndYear);
        Sut.RouteIntoSocialWork.Should().Be(journeyModel.RouteIntoSocialWork);
        Sut.OtherRouteIntoSocialWork.Should().Be(journeyModel.OtherRouteIntoSocialWork);
        Sut.OtherGenderIdentity.Should().Be(journeyModel.OtherGenderIdentity);

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-route-into-social-work");

        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetRegisterSocialWorkerJourneyModelAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetEscwRegisterChangeLinks(account.EthnicGroup), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/registration-complete");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.CompleteJourneyAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    private static string? GetOtherEthnicGroup(RegisterSocialWorkerJourneyModel? accountDetails)
    {
        return accountDetails?.OtherEthnicGroupWhite
               ?? accountDetails?.OtherEthnicGroupMixed
               ?? accountDetails?.OtherEthnicGroupAsian
               ?? accountDetails?.OtherEthnicGroupBlack
               ?? accountDetails?.OtherEthnicGroupOther;
    }
}
