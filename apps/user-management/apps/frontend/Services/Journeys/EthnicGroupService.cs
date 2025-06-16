using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EthnicGroupService(RegisterSocialWorkerJourneyService socialWorkerJourneyService) : IEthnicGroupService
{
    public async Task<EthnicGroup?> GetEthnicGroupAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.EthnicGroup;
    }

    public async Task SetEthnicGroupAsync(Guid accountId, EthnicGroup? ethnicGroup)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.EthnicGroup = ethnicGroup;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<EthnicGroupWhite?> GetEthnicGroupWhiteAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.EthnicGroupWhite;
    }

    public async Task SetEthnicGroupWhiteAsync(Guid accountId, EthnicGroupWhite? ethnicGroupWhite)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.EthnicGroupWhite = ethnicGroupWhite;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherEthnicGroupWhiteAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.OtherEthnicGroupWhite;
    }

    public async Task SetOtherEthnicGroupWhiteAsync(Guid accountId, string? ethnicGroupWhite)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.OtherEthnicGroupWhite = ethnicGroupWhite;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<EthnicGroupAsian?> GetEthnicGroupAsianAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.EthnicGroupAsian;
    }

    public async Task SetEthnicGroupAsianAsync(Guid accountId, EthnicGroupAsian? ethnicGroupAsian)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.EthnicGroupAsian = ethnicGroupAsian;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherEthnicGroupAsianAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.OtherEthnicGroupAsian;
    }

    public async Task SetOtherEthnicGroupAsianAsync(Guid accountId, string? ethnicGroupAsian)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.OtherEthnicGroupAsian = ethnicGroupAsian;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<EthnicGroupMixed?> GetEthnicGroupMixedAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.EthnicGroupMixed;
    }

    public async Task SetEthnicGroupMixedAsync(Guid accountId, EthnicGroupMixed? ethnicGroupMixed)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.EthnicGroupMixed = ethnicGroupMixed;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherEthnicGroupMixedAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.OtherEthnicGroupMixed;
    }

    public async Task SetOtherEthnicGroupMixedAsync(Guid accountId, string? ethnicGroupMixed)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.OtherEthnicGroupMixed = ethnicGroupMixed;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<EthnicGroupBlack?> GetEthnicGroupBlackAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.EthnicGroupBlack;
    }

    public async Task SetEthnicGroupBlackAsync(Guid accountId, EthnicGroupBlack? ethnicGroupBlack)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.EthnicGroupBlack = ethnicGroupBlack;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherEthnicGroupBlackAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.OtherEthnicGroupBlack;
    }

    public async Task SetOtherEthnicGroupBlackAsync(Guid accountId, string? ethnicGroupBlack)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.OtherEthnicGroupBlack = ethnicGroupBlack;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<EthnicGroupOther?> GetEthnicGroupOtherAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.EthnicGroupOther;
    }

    public async Task SetEthnicGroupOtherAsync(Guid accountId, EthnicGroupOther? ethnicGroupOther)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.EthnicGroupOther = ethnicGroupOther;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherEthnicGroupOtherAsync(Guid accountId)
    {
        var registerSocialWorkerJourneyModel = await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId);
        return registerSocialWorkerJourneyModel?.OtherEthnicGroupOther;
    }

    public async Task SetOtherEthnicGroupOtherAsync(Guid accountId, string? ethnicGroupOther)
    {
        var registerSocialWorkerJourneyModel =
            await socialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(accountId)
            ?? throw socialWorkerJourneyService.AccountNotFoundException(accountId);
        registerSocialWorkerJourneyModel.OtherEthnicGroupOther = ethnicGroupOther;
        socialWorkerJourneyService.SetRegisterSocialWorkerJourneyModel(accountId, registerSocialWorkerJourneyModel);
    }
}
