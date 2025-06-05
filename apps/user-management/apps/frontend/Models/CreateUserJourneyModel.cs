using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class CreateUserJourneyModel
{
    public ImmutableList<UserType>? UserTypes { get; set; }

    public UserDetails? UserDetails { get; set; }

    public bool? IsStaff { get; set; }

    public SocialWorker? SocialWorkerDetails { get; set; }

    public int? ExternalUserId { get; set; }

    public bool? IsRegisteredWithSocialWorkEngland { get; set; }

    public bool? IsStatutoryWorker { get; set; }

    public bool? IsAgencyWorker { get; set; }

    public bool? IsQualifiedWithin3Years { get; set; }

    public User ToUser()
    {
        return new User
        {
            Status =
                UserTypes != null
                && UserTypes.Contains(UserType.EarlyCareerSocialWorker)
                    ? UserStatus.PendingRegistration
                    : UserStatus.Active,
            Email = UserDetails?.Email,
            FirstName = UserDetails?.FirstName,
            LastName = UserDetails?.LastName,
            Types = UserTypes,
            SocialWorkEnglandNumber = UserDetails?.SocialWorkEnglandNumber,
            ExternalUserId = ExternalUserId,
            IsFunded = IsEligibleForFunding()
        };
    }

    private bool IsEligibleForFunding()
    {
        return UserTypes?.Contains(UserType.EarlyCareerSocialWorker) == true
               && IsRegisteredWithSocialWorkEngland == true
               && IsStatutoryWorker == true
               && IsAgencyWorker == false
               && IsQualifiedWithin3Years == true;
    }
}
