using System.Text.RegularExpressions;

namespace Dfe.Sww.Ecf.Frontend.Models;

public sealed class SocialWorkEnglandRecord
{
    private static readonly Regex SwePattern = new(
        pattern: @"^SW[1-9]\d{0,5}$",
        options: RegexOptions.IgnoreCase | RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromMilliseconds(250)
    );

    private readonly string _socialWorkEnglandNumber;
    private readonly DateOnly _statusCheckDueDate;

    /// <summary>
    /// Create a record with a Social Work England ID in the form "SW" + 1-6 digits.
    /// </summary>
    public SocialWorkEnglandRecord(string socialWorkEnglandNumber)
    {
        if (!IsValid(socialWorkEnglandNumber))
            throw new ArgumentException("Social Work England number must match pattern SW followed by 1 to 6 digits.", nameof(socialWorkEnglandNumber));

        _socialWorkEnglandNumber = Normalize(socialWorkEnglandNumber);
        _statusCheckDueDate = default;
    }

    /// <summary>
    /// Create a record with a Social Work England ID and due date.
    /// </summary>
    public SocialWorkEnglandRecord(string socialWorkEnglandNumber, DateOnly statusCheckDueDate)
    {
        if (!IsValid(socialWorkEnglandNumber))
            throw new ArgumentException("Social Work England number must match pattern SW followed by 1 to 6 digits.", nameof(socialWorkEnglandNumber));

        _socialWorkEnglandNumber = Normalize(socialWorkEnglandNumber);
        _statusCheckDueDate = statusCheckDueDate;
    }

    /// <summary>
    /// Returns the full ID string (e.g., "SW123456").
    /// </summary>
    public string GetNumber()
    {
        return _socialWorkEnglandNumber;
    }

    /// <summary>
    /// Get new instance with updated due date based on today's date incremented by provided number of days.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public SocialWorkEnglandRecord UpdateDueDate(int incrementDays)
    {
        if (incrementDays < 0)
            throw new ArgumentException("Increment value must not be negative", nameof(incrementDays));

        return new SocialWorkEnglandRecord(
            _socialWorkEnglandNumber,
            DateOnly.FromDateTime(DateTime.Now).AddDays(incrementDays)
        );
    }

    /// <summary>
    /// Parse a string into a SocialWorkEnglandRecord if it matches "SW" + 1-6 digits; throws on failure.
    /// </summary>
    public static SocialWorkEnglandRecord Parse(string sweId)
    {
        var isValid = TryParse(sweId, out var sweRecord);

        if (!isValid || sweRecord is null)
            throw new ArgumentException("Input must match pattern SW followed by 1 to 6 digits.", nameof(sweId));

        return sweRecord;
    }

    /// <summary>
    /// Tries to convert a Social Work England Registration ID (e.g., "SW123456")
    /// into its SocialWorkEnglandRecord equivalent.
    /// </summary>
    /// <returns><c>true</c> if converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? sweId, out SocialWorkEnglandRecord? socialWorkEnglandNumber)
    {
        socialWorkEnglandNumber = null;

        if (!IsValid(sweId))
            return false;

        socialWorkEnglandNumber = new SocialWorkEnglandRecord(Normalize(sweId!));
        return true;
    }

    /// <summary>
    /// Determine if Social Work England registration status check is due.
    /// </summary>
    public bool IsStatusCheckRequired()
    {
        return _statusCheckDueDate <= DateOnly.FromDateTime(DateTime.Now);
    }

    public override int GetHashCode()
    {
        // IDs are normalized to uppercase, so ordinal hash is fine
        return _socialWorkEnglandNumber.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not SocialWorkEnglandRecord other)
            return false;

        // Stored as normalized uppercase, so ordinal comparison is fine
        return _socialWorkEnglandNumber == other._socialWorkEnglandNumber;
    }

    public override string ToString()
    {
        return _socialWorkEnglandNumber;
    }

    private static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return SwePattern.IsMatch(value.Trim());
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToUpperInvariant();
    }
}
