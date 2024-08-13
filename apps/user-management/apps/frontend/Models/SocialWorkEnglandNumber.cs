namespace Dfe.Sww.Ecf.Frontend.Models;

public sealed class SocialWorkEnglandNumber
{
    private readonly int _socialWorkEnglandNumber;

    private readonly DateOnly _statusCheckDueDate;

    public SocialWorkEnglandNumber(int socialWorkEnglandNumber)
    {
        if (socialWorkEnglandNumber < 0)
        {
            throw new ArgumentException("Social Work England number must not be negative");
        }
        _socialWorkEnglandNumber = socialWorkEnglandNumber;
        _statusCheckDueDate = default;
    }

    public SocialWorkEnglandNumber(int socialWorkEnglandNumber, DateOnly statusCheckDueDate)
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
    public SocialWorkEnglandNumber UpdateDueDate(int incrementDays)
    {
        if (incrementDays > -1)
        {
            return new SocialWorkEnglandNumber(_socialWorkEnglandNumber, DateOnly.FromDateTime(DateTime.Now).AddDays(incrementDays));
        }
        throw new ArgumentException("Increment value must not be negative");
    }

    /// <summary>
    /// Return Social Work England Number object from string identifier
    /// </summary>
    /// <param name="sweId"></param>
    /// <returns></returns>
    public static SocialWorkEnglandNumber Parse(string sweId)
    {
        var sweNumber = sweId.StartsWith("SW") ? Int32.Parse(sweId.Remove(0, 2)) : Int32.Parse(sweId);
        return new SocialWorkEnglandNumber(sweNumber);
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
        if (obj is null || obj is not SocialWorkEnglandNumber account)
            return false;
        return _socialWorkEnglandNumber == account.GetNumber();
    }

    public override string ToString()
    {
        return $"{_socialWorkEnglandNumber.ToString()}";
    }
}
