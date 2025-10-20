namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface IAsyeSocialWorkerOperations
{
    Task<bool> ExistsAsync(string socialWorkerId);
}
