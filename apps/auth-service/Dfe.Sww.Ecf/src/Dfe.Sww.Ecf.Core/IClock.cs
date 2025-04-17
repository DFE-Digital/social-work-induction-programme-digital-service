namespace Dfe.Sww.Ecf.Core;

public interface IClock
{
    DateTime UtcNow { get; }
    DateOnly Today => DateOnly.FromDateTime(UtcNow);
}
