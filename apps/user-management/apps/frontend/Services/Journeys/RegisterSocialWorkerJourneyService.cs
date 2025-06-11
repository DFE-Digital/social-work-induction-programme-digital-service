using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class RegisterSocialWorkerJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IAccountService accountService
) : IRegisterSocialWorkerJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAccountService _accountService = accountService;

    private static string RegisterSocialWorkerSessionKey(Guid id) => "_registerSocialWorker-" + id;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private static KeyNotFoundException AccountNotFoundException(Guid id) =>
        new("Account not found with ID " + id);

    private async Task<RegisterSocialWorkerJourneyModel?> GetRegisterSocialWorkerJourneyModelAsync(Guid accountId)
    {
        Session.TryGet(
            RegisterSocialWorkerSessionKey(accountId),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );
        if (registerSocialWorkerJourneyModel is not null)
        {
            return registerSocialWorkerJourneyModel;
        }

        var account = await _accountService.GetByIdAsync(accountId);
        if (account is null)
        {
            return null;
        }

        registerSocialWorkerJourneyModel = new RegisterSocialWorkerJourneyModel(account);
        return registerSocialWorkerJourneyModel;
    }

    public async Task<DateTime?> GetDateOfBirthAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.DateOfBirth;
    }

    public async Task SetDateOfBirthAsync(Guid accountId, DateTime? dateOfBirth)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.DateOfBirth = dateOfBirth;
        SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task ResetRegisterSocialWorkerJourneyModel(Guid accountId)
    {
        var account = await _accountService.GetByIdAsync(accountId);
        if (account is null)
        {
            throw AccountNotFoundException(accountId);
        }

        Session.Remove(RegisterSocialWorkerSessionKey(accountId));
    }

    private void SetRegisterSocialWorkerJourneyModel(Guid accountId, RegisterSocialWorkerJourneyModel registerSocialWorkerJourneyModel)
    {
        Session.Set(RegisterSocialWorkerSessionKey(accountId), registerSocialWorkerJourneyModel);
    }

    public async Task<Account> CompleteJourneyAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);

        var updatedAccount = registerSocialWorkerJourneyModel.ToAccount();
        await _accountService.UpdateAsync(updatedAccount);

        await ResetRegisterSocialWorkerJourneyModel(accountId);
        return updatedAccount;
    }
}
