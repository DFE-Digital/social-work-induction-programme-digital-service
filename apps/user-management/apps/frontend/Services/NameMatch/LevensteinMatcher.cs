using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.NameMatch;

internal sealed class LevensteinMatcher : IMatcher
{
    /// <inheritdoc />
    public double MatchConfidence(string a, string b)
    {
        char[] sa;
        int n;
        int[] p; //'previous' cost array, horizontally
        int[] d; // cost array, horizontally
        int[] _d; //placeholder to assist in swapping p and d

        sa = b.ToCharArray();
        n = sa.Length;
        p = new int[n + 1];
        d = new int[n + 1];

        int m = a.Length;
        if (n == 0 || m == 0)
        {
            return 0;
        }

        // indexes into strings s and t
        int i; // iterates through s
        int j; // iterates through t

        char t_j; // jth character of t

        int cost; // cost

        for (i = 0; i <= n; i++)
        {
            p[i] = i;
        }

        for (j = 1; j <= m; j++)
        {
            t_j = a[j - 1];
            d[0] = j;

            for (i = 1; i <= n; i++)
            {
                cost = sa[i - 1] == t_j ? 0 : 1;
                // minimum of cell to the left+1, to the top+1, diagonally left and up +cost
                d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
            }

            // copy current distance counts to 'previous row' distance counts
            _d = p;
            p = d;
            d = _d;
        }

        // our last action in the above loop was to switch d and p, so p now
        // actually has the most recent cost counts
        return 1.0f - ((float)p[n] / Math.Max(a.Length, sa.Length));
    }
}
