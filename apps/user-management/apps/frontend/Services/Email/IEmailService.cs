using System.Threading.Tasks;

namespace Dfe.Sww.Ecf.Frontend.Services.Email;

public interface IEmailService
{
    Task SendInvitationEmailAsync(InvitationEmailRequest request);
}
