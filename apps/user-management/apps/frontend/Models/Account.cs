using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// User account
/// </summary>
public class Account
{
    /// <summary>
    /// First Name
    /// </summary>
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    /// <summary>
    /// FullName
    /// </summary>
    [Display(Name = "Full name")]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    /// <summary>
    /// Account status
    /// </summary>
    [Display(Name = "Status")]
    public AccountStatus? Status { get; set; }

    /// <summary>
    /// Account types
    /// </summary>
    [Display(Name = "Account type")]
    public List<AccountType>? Types { get; set; }

    /// <summary>
    /// Account ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }
}
