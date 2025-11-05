using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Email.Models;

public class WelcomeEmailRequest
{
    public Guid AccountId { get; init; }
}
