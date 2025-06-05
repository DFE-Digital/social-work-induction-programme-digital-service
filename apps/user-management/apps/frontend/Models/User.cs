using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// User account
/// </summary>
public record User
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Date and time the user was created
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
    [Display(Name = "Name")]
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; init; }

    /// <summary>
    /// User status
    /// </summary>
    [Display(Name = "Status")]
    public UserStatus? Status { get; init; }

    /// <summary>
    /// User types
    /// </summary>
    [Display(Name = "User type")]
    public ImmutableList<UserType>? Types { get; init; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; init; }

    public int? ExternalUserId { get; set; }

    public bool IsFunded { get; set; }

    public bool IsStaff =>
        Types?.Any(t => t is UserType.Coordinator or UserType.Assessor) ?? false;

    public User() { }

    public User(User user)
    {
        Id = user.Id;
        CreatedAt = user.CreatedAt;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Status = user.Status;
        Types = user.Types;
        SocialWorkEnglandNumber = user.SocialWorkEnglandNumber;
        ExternalUserId = user.ExternalUserId;
        IsFunded = user.IsFunded;
    }
}
