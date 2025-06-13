using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Repositories;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class AccountDetails
{
    [Display(Name = "First name")] public string? FirstName { get; init; }

    [Display(Name = "Middle names")] public string? MiddleNames { get; init; }

    [Display(Name = "Last name")] public string? LastName { get; init; }

    [Display(Name = "Full name")]
    public string FullName => string.Join(" ",
        new[]
            {
                FirstName,
                MiddleNames,
                LastName
            }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

    [Display(Name = "Email address")] public string? Email { get; init; }

    public string? SocialWorkEnglandNumber { get; init; }

    public DateOnly? ProgrammeStartDate { get; set; }

    public DateOnly? ProgrammeEndDate { get; set; }

    public int? ExternalUserId { get; set; }

    public bool IsFunded { get; set; }

    public bool IsStaff { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public static AccountDetails FromAccount(Account account)
    {
        return new AccountDetails
        {
            FirstName = account.FirstName,
            MiddleNames = account.MiddleNames,
            LastName = account.LastName,
            Email = account.Email,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
            ProgrammeStartDate = account.ProgrammeStartDate,
            ProgrammeEndDate = account.ProgrammeEndDate,
            ExternalUserId = account.ExternalUserId,
            IsFunded = account.IsFunded,
            IsStaff = account.IsStaff,
            DateOfBirth = account.DateOfBirth
        };
    }
}
