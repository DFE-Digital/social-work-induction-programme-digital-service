using Bogus;

namespace Dfe.Sww.Ecf.TestCommon.Extensions;

public static class BogusExtensions
{
    private static readonly char[] _ukNationalInsuranceFirstDigit =
        ['A', 'B', 'C', 'E', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'W', 'X', 'Z', 'Y'];

    private static readonly char[] _ukNationalInsuranceSecondDigit =
        ['A', 'B', 'C', 'E', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Z', 'Y'];

    private static readonly char[] _ukNationalInsuranceSuffix = ['A', 'B', 'C', 'D'];

    private static readonly string[] _ukNationalInsuranceNotAllowedPrefix = ["BG", "GB", "NK", "KN", "TN", "NT", "ZZ"];

    public static string UkNationalInsuranceNumber(this Person person, bool formatted = false)
    {
        string prefix;
        while (true)
        {
            prefix =
                $"{person.Random.ArrayElement(_ukNationalInsuranceFirstDigit)}{person.Random.ArrayElement(_ukNationalInsuranceSecondDigit)}";
            if (!_ukNationalInsuranceNotAllowedPrefix.Contains(prefix))
            {
                break;
            }
        }

        var number = string.Join("", person.Random.Digits(6));
        return formatted
            ? $"{prefix} {number[..2]} {number[2..4]} {number[4..6]} {person.Random.ArrayElement(_ukNationalInsuranceSuffix)}"
            : $"{prefix}{number}{person.Random.ArrayElement(_ukNationalInsuranceSuffix)}";
    }
}
