namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public abstract class UserBase
{
    public const int NameMaxLength = 200;
    public required Guid UserId { get; init; }
    public bool Active { get; set; } = true;
    public UserType UserType { get; }
    public required string Name { get; set; }
}

public class User : UserBase
{
    public required string? Email { get; set; }
    public required string[] Roles { get; set; }
}

public class SystemUser : UserBase
{
    public static Guid SystemUserId { get; } = new("a81394d1-a498-46d8-af3e-e077596ab303");
    public const string SystemUserName = "System";

    public static SystemUser Instance { get; } = new()
    {
        UserId = SystemUserId,
        Name = SystemUserName,
        Active = true
    };
}
