using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.NameMatch;

/// <summary>
/// Used for guidance, subject to change
/// </summary>
public enum MatchResult
{
    [Display(Name = "Fail")]
    Fail = 0,

    [Display(Name = "Poor")]
    Poor = 20,

    [Display(Name = "Average")]
    Average = 40,

    [Display(Name = "Good")]
    Good = 60,

    [Display(Name = "Very Good")]
    VeryGood = 80,

    [Display(Name = "Exact")]
    Exact = 100
}
