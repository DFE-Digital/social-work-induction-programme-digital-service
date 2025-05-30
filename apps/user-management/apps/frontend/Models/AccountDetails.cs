using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Repositories;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class AccountDetails
{
    /// <summary>
    /// First Name
    /// </summary>
    [Display(Name = "First name")]
    public string? FirstName { get; init; }

    /// <summary>
    /// Middle Names
    /// </summary>
    [Display(Name = "Middle names")]
    public string? MiddleNames { get; init; }

    /// <summary>
    /// Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string? LastName { get; init; }

    /// <summary>
    /// FullName
    /// </summary>
    [Display(Name = "Full name")]
    public string FullName => $"{FirstName} {MiddleNames} {LastName}".Trim();

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; init; }

    public string? SocialWorkEnglandNumber { get; init; }

    public int? ExternalUserId { get; set; }

    public bool IsStaff { get; set; }

    public static AccountDetails FromAccount(Account account)
    {
        return new AccountDetails
        {
            FirstName = account.FirstName,
            MiddleNames = account.MiddleNames,
            LastName = account.LastName,
            Email = account.Email,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
            ExternalUserId = account.ExternalUserId,
            IsStaff = account.IsStaff
        };
    }
}
