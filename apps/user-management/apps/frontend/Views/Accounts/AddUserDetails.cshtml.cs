using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Views.Accounts;

/// <summary>
/// Add User Details View Model
/// </summary>
public class AddUserDetailsModel
{
    /// <summary>
    /// First Name
    /// </summary>
    [Display(Name = "First name")]
    public string FirstName { get; init; } = null!;

    /// <summary>
    /// Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string LastName { get; init; } = null!;

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string Email { get; init; } = null!;

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }


    public static AddUserDetailsModel FromAccount(Account account)
    {
        return new AddUserDetailsModel
        {
            FirstName = account.FirstName,
            LastName = account.LastName,
            Email = account.Email,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber
        };
    }
}
