using Dfe.Sww.Ecf.Frontend.Services.EmailServices.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.EmailServices;

public class EmailService : IEmailService
{
    public EmailService(IPausingEmailService pausing)
    {
        Pausing = pausing;
    }

    public IPausingEmailService Pausing { get; init; }
}
