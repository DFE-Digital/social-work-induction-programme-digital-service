using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Validation.Common;

public static class CommonExtensions
{
    public static bool BeInTheFuture(YearMonth? yearMonth)
    {
        if (yearMonth is null)
        {
            return false;
        }

        var date = yearMonth.Value;

        var now = DateTime.UtcNow;
        var currentYearMonth = new YearMonth(now.Year, now.Month);
        return date > currentYearMonth;
    }

    public static bool BeInThePast(YearMonth? yearMonth)
    {
        if (yearMonth is null)
        {
            return false;
        }

        var date = yearMonth.Value;

        var now = DateTime.UtcNow;
        var currentYearMonth = new YearMonth(now.Year, now.Month);
        return date < currentYearMonth;
    }

    public static bool BeInThePast(LocalDate? localDate)
    {
        if (localDate is null)
        {
            return false;
        }

        var date = localDate.Value;

        var now = DateTime.UtcNow;
        var currentYearMonth = new LocalDate(now.Year, now.Month, now.Day);
        return date < currentYearMonth;
    }

    public static bool BeInTheFuture(LocalDate? localDate)
    {
        if (localDate is null)
        {
            return false;
        }

        var date = localDate.Value;

        var now = DateTime.UtcNow;
        var currentYearMonth = new LocalDate(now.Year, now.Month, now.Day);
        return date > currentYearMonth;
    }
}
