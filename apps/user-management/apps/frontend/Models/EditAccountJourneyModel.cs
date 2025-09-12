using System.Collections.Immutable;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class EditAccountJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public AccountStatus? AccountStatus { get; set; } = account.Status;

    public AccountDetails AccountDetails { get; set; } =
        new()
        {
            Id = account.Id,
            Types = account.Types,
            FirstName = account.FirstName,
            LastName = account.LastName,
            MiddleNames = account.MiddleNames,
            Email = account.Email,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
            IsStaff = account.IsStaff,
            ProgrammeStartDate = account.ProgrammeStartDate,
            ProgrammeEndDate = account.ProgrammeEndDate,
            ExternalUserId = account.ExternalUserId
        };

    public bool? IsStaff { get; set; } = account.IsStaff;

    public Account ToAccount()
    {
        return new Account(Account)
        {
            Email = AccountDetails.Email,
            FirstName = AccountDetails.FirstName,
            MiddleNames = AccountDetails.MiddleNames,
            LastName = AccountDetails.LastName,
            SocialWorkEnglandNumber = AccountDetails.SocialWorkEnglandNumber,
            Types = AccountDetails.Types?.ToImmutableList(),
            Status = AccountStatus,
            ProgrammeStartDate = AccountDetails.ProgrammeStartDate,
            ProgrammeEndDate = AccountDetails.ProgrammeEndDate,
            ExternalUserId = AccountDetails.ExternalUserId
        };
    }
}
