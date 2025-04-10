using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.NameMatch.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Services.NameMatch;

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
