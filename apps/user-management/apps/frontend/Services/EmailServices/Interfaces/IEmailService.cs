namespace Dfe.Sww.Ecf.Frontend.Services.EmailServices.Interfaces;

public interface IEmailService
{
    public IPausingEmailService Pausing { get; init; }
    public ILinkingEmailService Linking { get; init; }
}
