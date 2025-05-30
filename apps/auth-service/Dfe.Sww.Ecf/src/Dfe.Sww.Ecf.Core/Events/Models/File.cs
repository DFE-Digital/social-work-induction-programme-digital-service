namespace Dfe.Sww.Ecf.Core.Events.Models;

public record File
{
    public required Guid FileId { get; init; }
    public required string Name { get; init; }
}
