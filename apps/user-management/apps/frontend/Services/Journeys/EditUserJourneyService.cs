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

    private async Task<EditUserJourneyModel?> GetEditUserJourneyModelAsync(Guid userId)
    {
        Session.TryGet(
            EditUserSessionKey(userId),
            out EditUserJourneyModel? editUserJourneyModel
        );
        if (editUserJourneyModel is not null)
        {
            return editUserJourneyModel;
        }

        var user = await _userService.GetByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        editUserJourneyModel = new EditUserJourneyModel(user);
        return editUserJourneyModel;
    }

    public async Task<bool> IsUserIdValidAsync(Guid userId)
    {
        return await GetEditUserJourneyModelAsync(userId) is not null;
    }

    public async Task<ImmutableList<UserType>?> GetUserTypesAsync(Guid userId)
    {
        var editUserJourneyModel = await GetEditUserJourneyModelAsync(userId);
        return editUserJourneyModel?.UserTypes;
    }

    public async Task<UserDetails?> GetUserDetailsAsync(Guid userId)
    {
        var editUserJourneyModel = await GetEditUserJourneyModelAsync(userId);
        return editUserJourneyModel?.UserDetails;
    }

    public async Task<bool?> GetIsStaffAsync(Guid userId)
    {
        var editUserJourneyModel = await GetEditUserJourneyModelAsync(userId);
        return editUserJourneyModel?.IsStaff;
    }

    public async Task SetUserDetailsAsync(Guid userId, UserDetails userDetails)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(userId)
            ?? throw UserNotFoundException(userId);
        editUserJourneyModel.UserDetails = userDetails;
        SetEditUserJourneyModel(userId, editUserJourneyModel);
    }

    public async Task SetUserTypesAsync(Guid userId, IEnumerable<UserType> userTypes)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(userId)
            ?? throw UserNotFoundException(userId);
        editUserJourneyModel.UserTypes = userTypes.ToImmutableList();
        SetEditUserJourneyModel(userId, editUserJourneyModel);
    }

    public async Task SetUserStatusAsync(Guid userId, UserStatus userStatus)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(userId)
            ?? throw UserNotFoundException(userId);
        editUserJourneyModel.UserStatus = userStatus;
        SetEditUserJourneyModel(userId, editUserJourneyModel);
    }

    public async Task SetIsStaffAsync(Guid userId, bool? isStaff)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(userId)
            ?? throw UserNotFoundException(userId);
        editUserJourneyModel.IsStaff = isStaff;
        SetEditUserJourneyModel(userId, editUserJourneyModel);
    }

    public async Task ResetEditUserJourneyModelAsync(Guid userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user is null)
        {
            throw UserNotFoundException(userId);
        }

        Session.Remove(EditUserSessionKey(userId));
    }

    private void SetEditUserJourneyModel(
        Guid userId,
        EditUserJourneyModel? editUserJourneyModel
    )
    {
        Session.Set(EditUserSessionKey(userId), editUserJourneyModel);
    }

    public async Task<User> CompleteJourneyAsync(Guid userId)
    {
        var editUserJourneyModel =
            await GetEditUserJourneyModelAsync(userId)
            ?? throw UserNotFoundException(userId);

        var updatedUser = editUserJourneyModel.ToUser();
        await _userService.UpdateAsync(updatedUser);

        await ResetEditUserJourneyModelAsync(userId);
        return updatedUser;
    }
}
