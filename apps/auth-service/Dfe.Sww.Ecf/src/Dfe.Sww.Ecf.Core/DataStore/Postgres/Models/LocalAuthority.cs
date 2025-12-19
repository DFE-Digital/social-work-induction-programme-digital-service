using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class LocalAuthority
{
    [Key]
    [Required]
    public int OldLaCode { get; init; }

    [MaxLength(15)]
    public string? RegionCode { get; init; }

    [Required]
    [MaxLength(100)]
    public required string RegionName { get; init; }

    [Required]
    [MaxLength(100)]
    public required string LaName { get; init; }

    [MaxLength(15)]
    public string? NewLaCode { get; init; }
}
