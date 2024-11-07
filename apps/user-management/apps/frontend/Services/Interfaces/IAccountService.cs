using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface IAccountService
{
    public Task<List<Account>> GetAllAsync();
}
