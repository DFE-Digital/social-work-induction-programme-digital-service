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

    public async Task<DateOnly?> GetDateOfBirthAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.DateOfBirth;
    }

    public async Task SetDateOfBirthAsync(Guid accountId, DateOnly? dateOfBirth)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.DateOfBirth = dateOfBirth;
        SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<UserSex?> GetUserSexAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.UserSex;
    }

    public async Task SetUserSexAsync(Guid accountId, UserSex? userSex)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.UserSex = userSex;
        SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<GenderMatchesSexAtBirth?> GetUserGenderMatchesSexAtBirthAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.GenderMatchesSexAtBirth;
    }

    public async Task SetUserGenderMatchesSexAtBirthAsync(Guid accountId, GenderMatchesSexAtBirth? genderMatchesSexAtBirth)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.GenderMatchesSexAtBirth = genderMatchesSexAtBirth;
        SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherGenderIdentityAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.OtherGenderIdentity;
    }

    public async Task SetOtherGenderIdentityAsync(Guid accountId, string? otherGenderIdentity)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.OtherGenderIdentity = otherGenderIdentity;
        SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<EthnicGroup?> GetEthnicGroupAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.EthnicGroup;
    }

    public async Task SetEthnicGroupAsync(Guid accountId, EthnicGroup? ethnicGroup)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.EthnicGroup = ethnicGroup;
        SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public void ResetRegisterSocialWorkerJourneyModel(Guid accountId)
    {
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

        ResetRegisterSocialWorkerJourneyModel(accountId);
        return updatedAccount;
    }
}
