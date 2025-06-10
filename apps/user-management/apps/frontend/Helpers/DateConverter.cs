using System.Globalization;
using NodaTime;
using NodaTime.Text;

namespace Dfe.Sww.Ecf.Frontend.Helpers;

public class DateConverter
{
    /// <summary>
    /// Formats a NodaTime YearMonth into a "MonthName Year" string,
    /// e.g., "November 2023".
    /// </summary>
    /// <param name="yearMonth">The YearMonth object to format.</param>
    /// <returns>The formatted month and year string.</returns>
    public static string ToMonthYearString(YearMonth yearMonth)
    {
        var pattern = YearMonthPattern.Create("MMMM yyyy", CultureInfo.InvariantCulture);

        return pattern.Format(yearMonth);
    }
}
