using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EditUserJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IUserService userService
) : IEditUserJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUserService _userService = userService;

    private static string EditUserSessionKey(Guid id) => "_editUser-" + id;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private static KeyNotFoundException UserNotFoundException(Guid id) =>
        new("User not found with ID " + id);

    private async Task<EditUserJourneyModel?> GetEditUserJourneyModelAsync(Guid accountId)
    {
        Session.TryGet(
            EditUserSessionKey(accountId),
            out EditUserJourneyModel? editUserJourneyModel
        );
        if (editUserJourneyModel is not null)
        {
            return editUserJourneyModel;
        }

        var account = await _userService.GetByIdAsync(accountId);
        if (account is null)
        {
            return null;
        }

        editUserJourneyModel = new EditUserJourneyModel(account);
        return editUserJourneyModel;
    }

    public async Task<bool> IsUserIdValidAsync(Guid accountId)
    {
        return await GetEditUserJourneyModelAsync(accountId) is not null;
    }

    public async Task<ImmutableList<UserType>?> GetUserTypesAsync(Guid accountId)
    {
        var editUserJourneyModel = await GetEditUserJourneyModelAsync(accountId);
        return editUserJourneyModel?.UserTypes;
    }

    public async Task<UserDetails?> GetUserDetailsAsync(Guid accountId)
    {
        var editUserJourneyModel = await GetEditUserJourneyModelAsync(accountId);
        return editUserJourneyModel?.UserDetails;
    }

    public async Task<bool?> GetIsStaffAsync(Guid accountId)
    {
        var editUserJourneyModel = await GetEditUserJourneyModelAsync(accountId);
        return editUserJourneyModel?.IsStaff;
    }

    public async Task SetUserDetailsAsync(Guid accountId, UserDetails userDetails)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(accountId)
            ?? throw UserNotFoundException(accountId);
        editUserJourneyModel.UserDetails = userDetails;
        SetEditUserJourneyModel(accountId, editUserJourneyModel);
    }

    public async Task SetUserTypesAsync(Guid accountId, IEnumerable<UserType> accountTypes)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(accountId)
            ?? throw UserNotFoundException(accountId);
        editUserJourneyModel.UserTypes = accountTypes.ToImmutableList();
        SetEditUserJourneyModel(accountId, editUserJourneyModel);
    }

    public async Task SetUserStatusAsync(Guid accountId, UserStatus userStatus)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(accountId)
            ?? throw UserNotFoundException(accountId);
        editUserJourneyModel.UserStatus = userStatus;
        SetEditUserJourneyModel(accountId, editUserJourneyModel);
    }

    public async Task SetIsStaffAsync(Guid accountId, bool? isStaff)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(accountId)
            ?? throw UserNotFoundException(accountId);
        editUserJourneyModel.IsStaff = isStaff;
        SetEditUserJourneyModel(accountId, editUserJourneyModel);
    }

    public async Task ResetEditUserJourneyModelAsync(Guid accountId)
    {
        var account = await _userService.GetByIdAsync(accountId);
        if (account is null)
        {
            throw UserNotFoundException(accountId);
        }

        Session.Remove(EditUserSessionKey(accountId));
    }

    private void SetEditUserJourneyModel(
        Guid accountId,
        EditUserJourneyModel? editUserJourneyModel
    )
    {
        Session.Set(EditUserSessionKey(accountId), editUserJourneyModel);
    }

    public async Task<User> CompleteJourneyAsync(Guid accountId)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(accountId)
            ?? throw UserNotFoundException(accountId);

        var updatedUser = editUserJourneyModel.ToUser();
        await _userService.UpdateAsync(updatedUser);

        await ResetEditUserJourneyModelAsync(accountId);
        return updatedUser;
    }
}
