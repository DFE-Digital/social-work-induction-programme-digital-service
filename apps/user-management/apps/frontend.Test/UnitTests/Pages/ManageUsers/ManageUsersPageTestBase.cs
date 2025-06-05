using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public abstract class ManageUsersPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    private protected UserBuilder UserBuilder { get; }
    private protected UserDetailsFaker UserDetailsFaker { get; }

    private protected SocialWorkerFaker SocialWorkerFaker { get; }

    private protected CreateMoodleUserRequestFaker CreateMoodleUserRequestFaker { get; }

    private protected Mock<ICreateUserJourneyService> MockCreateUserJourneyService { get; }

    private protected Mock<IEditUserJourneyService> MockEditUserJourneyService { get; }

    private protected Mock<ISocialWorkEnglandService> MockSocialWorkEnglandService { get; }
    private protected Mock<IUserService> MockUserService { get; }

    private protected Mock<IMoodleServiceClient> MockMoodleServiceClient { get; }

    protected ManageUsersPageTestBase()
    {
        UserBuilder = new();
        UserDetailsFaker = new UserDetailsFaker();
        SocialWorkerFaker = new SocialWorkerFaker();

        MockCreateUserJourneyService = new();
        MockEditUserJourneyService = new();
        MockSocialWorkEnglandService = new();
        MockUserService = new();
        MockMoodleServiceClient = new();
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockCreateUserJourneyService.VerifyNoOtherCalls();
        MockEditUserJourneyService.VerifyNoOtherCalls();
        MockSocialWorkEnglandService.VerifyNoOtherCalls();
        MockUserService.VerifyNoOtherCalls();
        MockMoodleServiceClient.VerifyNoOtherCalls();
    }
}
