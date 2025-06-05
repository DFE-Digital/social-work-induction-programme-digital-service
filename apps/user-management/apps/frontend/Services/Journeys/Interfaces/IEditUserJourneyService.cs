using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IEditUserJourneyService
{
    Task<bool> IsUserIdValidAsync(Guid userTypes);
    Task<ImmutableList<UserType>?> GetUserTypesAsync(Guid userTypes);
    Task<UserDetails?> GetUserDetailsAsync(Guid userTypes);
    Task<bool?> GetIsStaffAsync(Guid userTypes);
    Task SetUserDetailsAsync(Guid userTypes, UserDetails userDetails);
    Task SetUserTypesAsync(Guid userId, IEnumerable<UserType> userTypes);
    Task SetUserStatusAsync(Guid userTypes, UserStatus userStatus);
    Task SetIsStaffAsync(Guid userId, bool? isStaff);
    Task ResetEditUserJourneyModelAsync(Guid userTypes);
    Task<User> CompleteJourneyAsync(Guid userTypes);
}
