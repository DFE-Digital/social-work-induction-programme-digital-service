using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.NameMatch;

internal sealed class ExactNameMatcher : IMatcher
{
    /// <inheritdoc />
    public double MatchConfidence(string a, string b)
    {
        var score = 0.0;

        if (b.Equals(a))
        {
            score = 1;
        }

        return score;
    }
}
