namespace Dfe.Sww.Ecf.Core;

public sealed class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
