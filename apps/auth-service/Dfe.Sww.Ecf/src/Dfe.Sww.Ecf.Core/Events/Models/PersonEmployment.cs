namespace Dfe.Sww.Ecf.Core.Events.Models;

public record PersonEmployment
{
    public required Guid PersonEmploymentId { get; init; }
    public required Guid PersonId { get; init; }
    public required Guid EstablishmentId { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly? EndDate { get; init; }

    public static PersonEmployment FromModel(DataStore.Postgres.Models.PersonOrganisation model) => new()
    {
        PersonEmploymentId = model.PersonOrganisationId,
        PersonId = model.PersonId,
        EstablishmentId = model.OrganisationId,
        StartDate = model.StartDate,
        EndDate = model.EndDate
    };
}
