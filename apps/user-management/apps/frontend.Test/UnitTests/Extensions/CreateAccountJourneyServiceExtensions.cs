using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;

public static class CreateUserJourneyServiceExtensions
{
    public static void PopulateJourneyModelFromUser(
        this ICreateUserJourneyService createUserJourneyService,
        User user
    )
    {
        createUserJourneyService.ResetCreateUserJourneyModel();

        createUserJourneyService.SetUserTypes(user.Types!);
        createUserJourneyService.SetUserDetails(UserDetails.FromUser(user));
        createUserJourneyService.SetIsStaff(
            user.Types?.Intersect([UserType.Assessor, UserType.Coordinator]).Any()
        );
    }
}
