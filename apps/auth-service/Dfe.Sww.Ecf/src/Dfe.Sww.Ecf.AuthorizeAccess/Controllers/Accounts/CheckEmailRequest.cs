using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;

public record CheckEmailRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid format.")]
    public string Email { get; init; } = string.Empty;
}
