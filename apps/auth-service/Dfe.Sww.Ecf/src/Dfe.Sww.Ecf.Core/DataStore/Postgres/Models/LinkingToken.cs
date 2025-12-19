using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class LinkingToken
{
    [Key]
    [Required]
    public Guid LinkingTokenId { get; set; }

    [Required]
    [ForeignKey("Person")]
    public Guid PersonId { get; init; }

    [Required]
    [StringLength(64)]
    public required string Token { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedOn { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? ExpirationOn { get; set; }

    // EF Navigation Properties
    public virtual Person? Person { get; set; }
}
