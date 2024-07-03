using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models;

/// <summary>
/// User account
/// </summary>
public class Account
{
    /// <summary>
    /// Full name of user
    /// </summary>
    [Display(Name = "Name")]
    public string? FullName { get; set; }

    /// <summary>
    /// Account status
    /// </summary>
    [Display(Name = "Status")]
    public string? Status { get; set; }

    /// <summary>
    /// Additional status-related data (optional)
    /// </summary>
    public string? AdditionalStatusInfo { get; set; }

    /// <summary>
    /// Account type
    /// </summary>
    [Display(Name = "Account type")]
    public List<string>? Type { get; set; }

    /// <summary>
    /// Account ID
    /// </summary>
    public int Id { get; set; }
}
