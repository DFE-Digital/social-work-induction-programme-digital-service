using System.Threading.Tasks;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Email;

public interface IEmailService
{
    Task SendInvitationEmailAsync(InvitationEmailRequest request);
    Task SendWelcomeEmailAsync(WelcomeEmailRequest request);
}
