using System.Collections.Immutable;
using static Dfe.Sww.Ecf.Frontend.Models.UserStatus;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class EditUserJourneyModel(User user)
{
    public User User { get; } = user;

    public ImmutableList<UserType>? UserTypes { get; set; } = user.Types;
    public UserStatus? UserStatus { get; set; } = user.Status;

    public UserDetails UserDetails { get; set; } =
        new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            SocialWorkEnglandNumber = user.SocialWorkEnglandNumber,
            IsStaff = user.IsStaff
        };

    public bool? IsStaff { get; set; } = user.IsStaff;

    public User ToUser()
    {
        return new User(User)
        {
            Email = UserDetails.Email,
            FirstName = UserDetails.FirstName,
            LastName = UserDetails.LastName,
            SocialWorkEnglandNumber = UserDetails.SocialWorkEnglandNumber,
            Types = UserTypes,
            Status = UserStatus
        };
    }
}
