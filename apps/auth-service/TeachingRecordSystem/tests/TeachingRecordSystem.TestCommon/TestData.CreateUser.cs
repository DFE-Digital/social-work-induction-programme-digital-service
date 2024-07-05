using TeachingRecordSystem.Core.DataStore.Postgres.Models;

namespace TeachingRecordSystem.TestCommon;

public partial class TestData
{
    public async Task<User> CreateUser(
        bool? active = null,
        string? name = null,
        string? email = null,
        string[]? roles = null,
        Guid? azureAdUserId = null)
    {
        var user = await WithDbContext(async dbContext =>
        {
            active ??= true;
            name ??= GenerateName();
            email ??= GenerateUniqueEmail();
            roles ??= [UserRoles.Helpdesk];

            var user = new User()
            {
                Active = active.Value,
                Name = name,
                Email = email,
                Roles = roles,
                UserId = Guid.NewGuid(),
                AzureAdUserId = azureAdUserId?.ToString(),
            };

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();

            return user;
        });

        return user;
    }
}
