using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// User account
/// </summary>
public record Account
{
    /// <summary>
    /// Account ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Date and time the account was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// First Name
    /// </summary>
    [Display(Name = "First name")]
    public string? FirstName { get; init; }

    /// <summary>
    /// Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string? LastName { get; init; }

    /// <summary>
    /// FullName
    /// </summary>
    [Display(Name = "Full name")]
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; init; }

    /// <summary>
    /// Account status
    /// </summary>
    [Display(Name = "Status")]
    public AccountStatus? Status { get; init; }

    /// <summary>
    /// Account types
    /// </summary>
    [Display(Name = "Account type")]
    public ImmutableList<AccountType>? Types { get; init; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; init; }

    public int? ExternalUserId { get; set; }

    public bool IsFunded { get; set; }

    public bool IsStaff =>
        Types?.Any(t => t is AccountType.Coordinator or AccountType.Assessor) ?? false;

    public Account() { }

    public Account(Account account)
    {
        Id = account.Id;
        CreatedAt = account.CreatedAt;
        FirstName = account.FirstName;
        LastName = account.LastName;
        Email = account.Email;
        Status = account.Status;
        Types = account.Types;
        SocialWorkEnglandNumber = account.SocialWorkEnglandNumber;
        ExternalUserId = account.ExternalUserId;
        IsFunded = account.IsFunded;
    }
}
