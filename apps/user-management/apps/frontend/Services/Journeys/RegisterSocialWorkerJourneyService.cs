using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class RegisterSocialWorkerJourneyService : IRegisterSocialWorkerJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccountService _accountService;
    private readonly EcfLinkGenerator _ecfLinkGenerator;
    private readonly IEmailService _emailService;
    public IEthnicGroupService EthnicGroups { get; init; }

    public RegisterSocialWorkerJourneyService(
        IHttpContextAccessor httpContextAccessor,
        IAccountService accountService,
        EcfLinkGenerator ecfLinkGenerator,
        IEmailService emailService)
    {
        _httpContextAccessor = httpContextAccessor;
        _accountService = accountService;
        _ecfLinkGenerator = ecfLinkGenerator;
        _emailService = emailService;
        EthnicGroups = new EthnicGroupService(this);
    }

    private static string RegisterSocialWorkerSessionKey(Guid id) => "_registerSocialWorker-" + id;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    protected internal KeyNotFoundException AccountNotFoundException(Guid id) =>
        new("Account not found with ID " + id);

    public async Task<RegisterSocialWorkerJourneyModel?> GetRegisterSocialWorkerJourneyModelAsync(Guid personId)
    {
        Session.TryGet(
            RegisterSocialWorkerSessionKey(personId),
            out RegisterSocialWorkerJourneyModel? registerSocialWorkerJourneyModel
        );
        if (registerSocialWorkerJourneyModel is not null)
        {
            return registerSocialWorkerJourneyModel;
        }

        var account = await _accountService.GetByIdAsync(personId);
        if (account is null)
        {
            return null;
        }

        registerSocialWorkerJourneyModel = new RegisterSocialWorkerJourneyModel(account);
        return registerSocialWorkerJourneyModel;
    }

    public async Task<DateOnly?> GetDateOfBirthAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.DateOfBirth;
    }

    public async Task SetDateOfBirthAsync(Guid personId, DateOnly? dateOfBirth)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.DateOfBirth = dateOfBirth;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<UserSex?> GetUserSexAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.UserSex;
    }

    public async Task SetUserSexAsync(Guid personId, UserSex? userSex)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.UserSex = userSex;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<GenderMatchesSexAtBirth?> GetUserGenderMatchesSexAtBirthAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.GenderMatchesSexAtBirth;
    }

    public async Task SetUserGenderMatchesSexAtBirthAsync(Guid personId, GenderMatchesSexAtBirth? genderMatchesSexAtBirth)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.GenderMatchesSexAtBirth = genderMatchesSexAtBirth;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherGenderIdentityAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.OtherGenderIdentity;
    }

    public async Task SetOtherGenderIdentityAsync(Guid personId, string? otherGenderIdentity)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.OtherGenderIdentity = otherGenderIdentity;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<Disability?> GetIsDisabledAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.Disability;
    }

    public async Task SetIsDisabledAsync(Guid personId, Disability? isDisabled)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.Disability = isDisabled;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<DateOnly?> GetSocialWorkEnglandRegistrationDateAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.SocialWorkEnglandRegistrationDate;
    }

    public async Task SetSocialWorkEnglandRegistrationDateAsync(Guid personId, DateOnly? socialWorkEnglandRegistrationDate)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.SocialWorkEnglandRegistrationDate = socialWorkEnglandRegistrationDate;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<Qualification?> GetHighestQualificationAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.HighestQualification;
    }

    public async Task SetHighestQualificationAsync(Guid personId, Qualification? highestQualification)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.HighestQualification = highestQualification;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<int?> GetSocialWorkQualificationEndYearAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.SocialWorkQualificationEndYear;
    }

    public async Task SetSocialWorkQualificationEndYearAsync(Guid personId, int? socialWorkEnglandQualificationEndYear)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.SocialWorkQualificationEndYear = socialWorkEnglandQualificationEndYear;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<RouteIntoSocialWork?> GetRouteIntoSocialWorkAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.RouteIntoSocialWork;
    }

    public async Task SetRouteIntoSocialWorkAsync(Guid personId, RouteIntoSocialWork? routeIntoSocialWork)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.RouteIntoSocialWork = routeIntoSocialWork;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public async Task<string?> GetOtherRouteIntoSocialWorkAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel = await GetRegisterSocialWorkerJourneyModelAsync(personId);
        return registerSocialWorkerJourneyModel?.OtherRouteIntoSocialWork;
    }

    public async Task SetOtherRouteIntoSocialWorkAsync(Guid personId, string? otherRouteIntoSocialWorkAsync)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);
        registerSocialWorkerJourneyModel.OtherRouteIntoSocialWork = otherRouteIntoSocialWorkAsync;
        SetRegisterSocialWorkerJourneyModel(personId, registerSocialWorkerJourneyModel);
    }

    public EscwRegisterChangeLinks GetEscwRegisterChangeLinks(EthnicGroup? ethnicGroup)
    {
        return new EscwRegisterChangeLinks
        {
            DateOfBirthChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationDateOfBirthChange(),
            UserSexChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationSexAndGenderIdentityChange(),
            GenderIdentityChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationSexAndGenderIdentityChange(),
            EthnicGroupChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupChange(),
            EthnicGroupingChangeLink = ethnicGroup switch
            {
                EthnicGroup.White => _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupWhiteChange(),
                EthnicGroup.MixedOrMultipleEthnicGroups => _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupMixedChange(),
                EthnicGroup.AsianOrAsianBritish => _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupAsianChange(),
                EthnicGroup.BlackAfricanCaribbeanOrBlackBritish => _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupBlackChange(),
                EthnicGroup.OtherEthnicGroup => _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupOtherChange(),
                EthnicGroup.PreferNotToSay => _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupChange(),
                _ => _ecfLinkGenerator.SocialWorkerRegistrationEthnicGroupChange()
            },
            DisabilityChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationSelectDisabilityChange(),
            SocialWorkEnglandRegistrationChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationSelectSocialWorkEnglandRegistrationDateChange(),
            HighestQualificationChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationSelectHighestQualificationChange(),
            SocialWorkQualificationEndYearChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationSelectSocialWorkQualificationEndYearChange(),
            RouteIntoSocialWorkChangeLink = _ecfLinkGenerator.SocialWorkerRegistrationSelectRouteIntoSocialWorkChange()
        };
    }

    public void ResetRegisterSocialWorkerJourneyModel(Guid personId)
    {
        Session.Remove(RegisterSocialWorkerSessionKey(personId));
    }

    protected internal void SetRegisterSocialWorkerJourneyModel(Guid personId, RegisterSocialWorkerJourneyModel registerSocialWorkerJourneyModel)
    {
        Session.Set(RegisterSocialWorkerSessionKey(personId), registerSocialWorkerJourneyModel);
    }

    public async Task<Account> CompleteJourneyAsync(Guid personId)
    {
        var registerSocialWorkerJourneyModel =
            await GetRegisterSocialWorkerJourneyModelAsync(personId)
            ?? throw AccountNotFoundException(personId);

        var updatedAccount = registerSocialWorkerJourneyModel.Account;

        updatedAccount.DateOfBirth = registerSocialWorkerJourneyModel.DateOfBirth;
        updatedAccount.UserSex = registerSocialWorkerJourneyModel.UserSex;
        updatedAccount.GenderMatchesSexAtBirth = registerSocialWorkerJourneyModel.GenderMatchesSexAtBirth;
        updatedAccount.OtherGenderIdentity = registerSocialWorkerJourneyModel.OtherGenderIdentity;
        updatedAccount.EthnicGroup = registerSocialWorkerJourneyModel.EthnicGroup;
        updatedAccount.EthnicGroupWhite = registerSocialWorkerJourneyModel.EthnicGroupWhite;
        updatedAccount.OtherEthnicGroupWhite = registerSocialWorkerJourneyModel.OtherEthnicGroupWhite;
        updatedAccount.EthnicGroupMixed = registerSocialWorkerJourneyModel.EthnicGroupMixed;
        updatedAccount.OtherEthnicGroupMixed = registerSocialWorkerJourneyModel.OtherEthnicGroupMixed;
        updatedAccount.EthnicGroupAsian = registerSocialWorkerJourneyModel.EthnicGroupAsian;
        updatedAccount.OtherEthnicGroupAsian = registerSocialWorkerJourneyModel.OtherEthnicGroupAsian;
        updatedAccount.EthnicGroupBlack = registerSocialWorkerJourneyModel.EthnicGroupBlack;
        updatedAccount.OtherEthnicGroupBlack = registerSocialWorkerJourneyModel.OtherEthnicGroupBlack;
        updatedAccount.EthnicGroupOther = registerSocialWorkerJourneyModel.EthnicGroupOther;
        updatedAccount.OtherEthnicGroupOther = registerSocialWorkerJourneyModel.OtherEthnicGroupOther;
        updatedAccount.Disability = registerSocialWorkerJourneyModel.Disability;
        updatedAccount.SocialWorkEnglandRegistrationDate = registerSocialWorkerJourneyModel.SocialWorkEnglandRegistrationDate;
        updatedAccount.HighestQualification = registerSocialWorkerJourneyModel.HighestQualification;
        updatedAccount.SocialWorkQualificationEndYear = registerSocialWorkerJourneyModel.SocialWorkQualificationEndYear;
        updatedAccount.RouteIntoSocialWork = registerSocialWorkerJourneyModel.RouteIntoSocialWork;
        updatedAccount.OtherRouteIntoSocialWork = registerSocialWorkerJourneyModel.OtherRouteIntoSocialWork;
        updatedAccount.Status = AccountStatus.Active;

        await _accountService.UpdateAsync(updatedAccount);

        await _emailService.SendWelcomeEmailAsync(new WelcomeEmailRequest
        {
            AccountId = personId
        });

        ResetRegisterSocialWorkerJourneyModel(personId);
        return updatedAccount;
    }
}
