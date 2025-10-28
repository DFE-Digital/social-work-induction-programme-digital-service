using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class CreateAccountJourneyModel
{
    public ImmutableList<AccountType>? AccountTypes { get; set; }

    public AccountDetails? AccountDetails { get; set; }

    public bool? IsStaff { get; set; }

    public SocialWorker? SocialWorkerDetails { get; set; }

    public int? ExternalUserId { get; set; }

    public bool? IsRegisteredWithSocialWorkEngland { get; set; }

    public bool? IsStatutoryWorker { get; set; }

    public bool? IsAgencyWorker { get; set; }

    /// <summary>
    /// Property capturing whether the Social Worker ID has already been enrolled on ASYE
    /// </summary>
    public bool? IsEnrolledInAsye { get; set; }

    /// <summary>
    /// Property capturing whether the user has completed their social work qualification within the last 3 years.
    /// </summary>
    public bool? IsRecentlyQualified { get; set; }

    public DateOnly? ProgrammeStartDate { get; set; }

    public DateOnly? ProgrammeEndDate { get; set; }
    public bool? IsFunded { get; set; }

    public Account ToAccount()
    {
        return new Account
        {
            Status = AccountStatus.PendingRegistration,
            Email = AccountDetails?.Email,
            FirstName = AccountDetails?.FirstName,
            MiddleNames = AccountDetails?.MiddleNames,
            LastName = AccountDetails?.LastName,
            Types = AccountTypes,
            SocialWorkEnglandNumber = AccountDetails?.SocialWorkEnglandNumber,
            ProgrammeStartDate = ProgrammeStartDate,
            ProgrammeEndDate = ProgrammeEndDate,
            ExternalUserId = ExternalUserId,
            IsFunded = IsEligibleForFunding()
        };
    }

    public bool IsEligibleForFunding()
    {
        return AccountTypes?.Contains(AccountType.EarlyCareerSocialWorker) == true
            && IsRegisteredWithSocialWorkEngland == true
            && IsStatutoryWorker == true
            && IsAgencyWorker == false
            && IsRecentlyQualified == true
            && IsEnrolledInAsye == false;
    }
}
