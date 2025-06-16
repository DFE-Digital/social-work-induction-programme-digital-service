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

    public bool? IsQualifiedWithin3Years { get; set; }

    public DateOnly? ProgrammeStartDate { get; set; }

    public DateOnly? ProgrammeEndDate { get; set; }

    public Account ToAccount()
    {
        return new Account
        {
            Status =
                AccountTypes != null
                && AccountTypes.Contains(AccountType.EarlyCareerSocialWorker)
                    ? AccountStatus.PendingRegistration
                    : AccountStatus.Active,
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

    private bool IsEligibleForFunding()
    {
        return AccountTypes?.Contains(AccountType.EarlyCareerSocialWorker) == true
            && IsRegisteredWithSocialWorkEngland == true
            && IsStatutoryWorker == true
            && IsAgencyWorker == false
            && IsQualifiedWithin3Years == true;
    }
}
