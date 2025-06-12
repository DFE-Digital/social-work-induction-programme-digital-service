namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface IHttpContextService
{
    Guid GetPersonId();
    string GetOrganisationId();
    bool GetIsEcswRegistered();
}
