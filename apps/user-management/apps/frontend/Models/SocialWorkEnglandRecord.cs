namespace Dfe.Sww.Ecf.Frontend.Models;

public sealed class SocialWorkEnglandRecord
{
    private readonly int _socialWorkEnglandNumber;

    private readonly DateOnly _statusCheckDueDate;

    public SocialWorkEnglandRecord(int socialWorkEnglandNumber)
    {
        if (socialWorkEnglandNumber < 0)
        {
            throw new ArgumentException("Social Work England number must not be negative");
        }
        _socialWorkEnglandNumber = socialWorkEnglandNumber;
        _statusCheckDueDate = default;
    }

    public SocialWorkEnglandRecord(int socialWorkEnglandNumber, DateOnly statusCheckDueDate)
    {
        if (socialWorkEnglandNumber < 0)
        {
            throw new ArgumentException("Social Work England number must not be negative");
        }
        _socialWorkEnglandNumber = socialWorkEnglandNumber;
        _statusCheckDueDate = statusCheckDueDate;
    }

    public int GetNumber()
    {
        return _socialWorkEnglandNumber;
    }

    /// <summary>
    /// Get new instance with updated due date
    /// based on today's date incremented by provided number of days
    /// </summary>
    /// <param name="incrementDays"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public SocialWorkEnglandRecord UpdateDueDate(int incrementDays)
    {
        if (incrementDays > -1)
        {
            return new SocialWorkEnglandRecord(
                _socialWorkEnglandNumber,
                DateOnly.FromDateTime(DateTime.Now).AddDays(incrementDays)
            );
        }
        throw new ArgumentException("Increment value must not be negative");
    }

    /// <summary>
    /// Return Social Work England Number object from string identifier
    /// </summary>
    /// <param name="sweId"></param>
    /// <returns></returns>
    public static SocialWorkEnglandRecord Parse(string sweId)
    {
        var sweNumber = sweId.StartsWith("SW", StringComparison.OrdinalIgnoreCase)
            ? int.Parse(sweId.Remove(0, 2))
            : int.Parse(sweId);
        return new SocialWorkEnglandRecord(sweNumber);
    }

    /// <summary>
    /// Tries to convert a Social Work England Registration ID into its Social Work England Number object equivalent
    /// </summary>
    /// <param name="sweId"></param>
    /// <param name="socialWorkEnglandNumber"></param>
    /// <returns><c>true</c> if <paramref name="sweId" /> was converted successfully; otherwise, false.</returns>
    public static bool TryParse(string? sweId, out SocialWorkEnglandRecord? socialWorkEnglandNumber)
    {
        if (string.IsNullOrWhiteSpace(sweId))
        {
            socialWorkEnglandNumber = null;
            return false;
        }

        if (sweId.StartsWith("SW", StringComparison.InvariantCultureIgnoreCase))
        {
            sweId = sweId.Remove(0, 2);
        }

        var startWithZero = sweId.FirstOrDefault() == '0';
        if (startWithZero)
        {
            socialWorkEnglandNumber = null;
            return false;
        }

        var isNumber = int.TryParse(sweId, out var sweNumber);
        if (!isNumber || sweNumber <= 0)
        {
            socialWorkEnglandNumber = null;
            return false;
        }

        var isCorrectLength = sweId.Trim().Length <= 6;
        if (isCorrectLength == false)
        {
            socialWorkEnglandNumber = null;
            return false;
        }

        socialWorkEnglandNumber = new SocialWorkEnglandRecord(sweNumber);
        return true;
    }

    /// <summary>
    /// Determine if Social Work England registration status check is due
    /// </summary>
    /// <returns>boolean for whether a check is needed</returns>
    public bool IsStatusCheckRequired()
    {
        return _statusCheckDueDate <= DateOnly.FromDateTime(DateTime.Now);
    }

    public override int GetHashCode()
    {
        return _socialWorkEnglandNumber;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not SocialWorkEnglandRecord account)
            return false;
        return _socialWorkEnglandNumber == account.GetNumber();
    }

    public override string ToString()
    {
        return $"{_socialWorkEnglandNumber}";
    }
}
