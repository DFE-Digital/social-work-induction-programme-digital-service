using System.Text.Json;
using System.Text.Json.Nodes;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.TestCommon;

public partial class TestData
{
    public Task<OneLoginUser> CreateOneLoginUser(CreatePersonResult createPersonResult, string? subject = null,
        string? email = null)
    {
        return CreateOneLoginUser(
            createPersonResult.PersonId,
            subject,
            email,
            ([createPersonResult.FirstName, createPersonResult.LastName], createPersonResult.DateOfBirth));
    }

    public Task<OneLoginUser> CreateOneLoginUser(string? subject = null, string? email = null, bool verified = false)
    {
        return CreateOneLoginUser(
            null,
            subject,
            email,
            verified
                ? ([_faker.Person.FirstName, _faker.Person.LastName],
                    DateOnly.FromDateTime(_faker.Person.DateOfBirth))
                : null);
    }

    public Task<OneLoginUser> CreateOneLoginUser(
        Guid? personId,
        string? subject = null,
        string? email = null,
        (string[] Name, DateOnly DateOfBirth)? verifiedInfo = null)
    {
        if (personId is not null && verifiedInfo is null)
        {
            throw new ArgumentException("OneLoginUser with a Person must be verified.", nameof(verifiedInfo));
        }

        return WithDbContext(async dbContext =>
        {
            subject ??= CreateOneLoginUserSubject();
            email ??= _faker.Internet.Email();

            var user = new OneLoginUser
            {
                Subject = subject,
                Email = email,
                FirstOneLoginSignIn = Clock.UtcNow,
                LastOneLoginSignIn = Clock.UtcNow,
                PersonId = personId
            };

            if (verifiedInfo is not null)
            {
                user.VerifiedOn = Clock.UtcNow;
                user.VerificationRoute = OneLoginUserVerificationRoute.OneLogin;
                user.VerifiedNames = [verifiedInfo.Value.Name];
                user.VerifiedDatesOfBirth = [verifiedInfo.Value.DateOfBirth];
            }

            dbContext.OneLoginUsers.Add(user);

            await dbContext.SaveChangesAsync();

            return user;
        });
    }

    public static string CreateOneLoginUserSubject()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static JsonDocument CreateOneLoginCoreIdentityVc(string firstName, string lastName, DateOnly dateOfBirth)
    {
        return JsonDocument.Parse(
            new JsonObject
            {
                ["type"] = new JsonArray(
                    JsonValue.Create("VerifiableCredential"),
                    JsonValue.Create("IdentityCheckCredential")),
                ["aud"] = "test_client_id",
                ["credentialSubject"] = new JsonObject
                {
                    ["name"] = new JsonArray(
                        new JsonObject
                        {
                            ["nameParts"] = new JsonArray(
                                new JsonObject
                                {
                                    ["value"] = firstName,
                                    ["type"] = "GivenName"
                                },
                                new JsonObject
                                {
                                    ["value"] = lastName,
                                    ["type"] = "FamilyName"
                                })
                        }),
                    ["birthDate"] = new JsonArray(
                        new JsonObject
                        {
                            ["value"] = dateOfBirth.ToString("yyyy-MM-dd")
                        })
                }
            }.ToString());
    }
}
