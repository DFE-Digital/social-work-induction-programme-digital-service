using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Mappers;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class AccountService(
    IAuthServiceClient authServiceClient,
    IModelMapper<Person, Account> mapper
) : IAccountService
{
    public async Task<PaginationResult<Account>> GetAllAsync(PaginationRequest request)
    {
        var persons = await authServiceClient.Accounts.GetAllAsync(request);

        var accounts = new PaginationResult<Account>
        {
            Records = persons.Records.Select(mapper.MapToBo).ToList(),
            MetaData = persons.MetaData
        };

        return accounts;
    }
}
