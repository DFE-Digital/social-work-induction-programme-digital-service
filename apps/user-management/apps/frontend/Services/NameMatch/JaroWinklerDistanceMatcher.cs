using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.NameMatch.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.NameMatch;

internal sealed class JaroWinklerDistanceMatcher : IMatcher
{
    private readonly double _threshold = 0.7d;

    /// <inheritdoc />
    public double MatchConfidence(string a, string b)
    {
        var mtp = Matches(a, b);
        double m = mtp[0];
        if (m == 0)
        {
            return 0d;
        }
        var j = ((m / a.Length + m / b.Length + (m - mtp[1]) / m)) / 3;
        var jw = j < _threshold ? j : j + Math.Min(0.1d, 1d / mtp[3]) * mtp[2] * (1 - j);
        return jw;
    }

    private static int[] Matches(string s1, string s2)
    {
        string max,
            min;

        if (s1.Length > s2.Length)
        {
            max = s1;
            min = s2;
        }
        else
        {
            max = s2;
            min = s1;
        }
        var range = Math.Max(max.Length / 2 - 1, 0);
        var matchIndexes = new int[min.Length];
        Array.Fill(matchIndexes, -1);
        var matchFlags = new bool[max.Length];
        var matches = 0;
        for (var mi = 0; mi < min.Length; mi++)
        {
            var c1 = min[mi];
            for (
                int xi = Math.Max(mi - range, 0), xn = Math.Min(mi + range + 1, max.Length);
                xi < xn;
                xi++
            )
            {
                if (!matchFlags[xi] && c1 == max[xi])
                {
                    matchIndexes[mi] = xi;
                    matchFlags[xi] = true;
                    matches++;
                    break;
                }
            }
        }
        var ms1 = new char[matches];
        var ms2 = new char[matches];
        for (int i = 0, si = 0; i < min.Length; i++)
        {
            if (matchIndexes[i] != -1)
            {
                ms1[si] = min[i];
                si++;
            }
        }
        for (int i = 0, si = 0; i < max.Length; i++)
        {
            if (matchFlags[i])
            {
                ms2[si] = max[i];
                si++;
            }
        }
        var transpositions = 0;
        for (var mi = 0; mi < ms1.Length; mi++)
        {
            if (ms1[mi] != ms2[mi])
            {
                transpositions++;
            }
        }
        var prefix = 0;
        for (var mi = 0; mi < min.Length; mi++)
        {
            if (s1[mi] == s2[mi])
            {
                prefix++;
            }
            else
            {
                break;
            }
        }
        return [matches, transpositions / 2, prefix, max.Length];
    }
}
