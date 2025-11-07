using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface IMoodleService
{
    Task<int?> CreateCourseAsync(Organisation organisation);
}
