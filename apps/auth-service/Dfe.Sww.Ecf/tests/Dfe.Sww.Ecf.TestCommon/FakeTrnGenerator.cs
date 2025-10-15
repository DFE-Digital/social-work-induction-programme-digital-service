namespace Dfe.Sww.Ecf.TestCommon;

public class FakeTrnGenerator
{
    private int _lastTrn = 40000000;

    public string GenerateTrn() => Interlocked.Increment(ref _lastTrn).ToString();
}
