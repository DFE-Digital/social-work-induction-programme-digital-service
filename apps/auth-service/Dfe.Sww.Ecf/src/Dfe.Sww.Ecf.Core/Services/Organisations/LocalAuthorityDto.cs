using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Hangfire.Annotations;

namespace Dfe.Sww.Ecf.Core.Services.Organisations;

[PublicAPI]
public class LocalAuthorityDto
{
    public required string OrganisationName { get; set; }
    public int LocalAuthorityCode { get; set; }
    public required string Region { get; set; }
}

public static class LocalAuthorityDtoExtensions
{
    public static LocalAuthorityDto ToDto(this LocalAuthority localAuthority) =>
        new()
        {
            OrganisationName = localAuthority.LaName,
            LocalAuthorityCode = localAuthority.OldLaCode,
            Region = localAuthority.RegionName,
        };
}
