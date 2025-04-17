using Dfe.Sww.Ecf.Frontend.Models.NameMatch;

namespace Dfe.Sww.Ecf.Frontend.Services.NameMatch.Interfaces;

/// <summary>
/// Validation service for social workers
/// </summary>
public interface ISocialWorkerValidatorService
{
    /// <summary>
    /// Validates the social worker name provided against the name retrieved from the SWE API
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="fullName"></param>
    /// <returns>A <see cref="MatchResult"/> value from the average result of all the algorithms ran</returns>
    /// <exception cref="ArgumentException">Any input parameters passed into the method are null</exception>
    /// <exception cref="ArgumentException">If both <see cref="firstName"/> and <see cref="lastName"/> are empty or whitespace</exception>
    MatchResult ConvertToResult(string firstName, string lastName, string fullName);

    /// <seealso cref="ConvertToResult(string,string,string)"/>
    MatchResult ConvertToResult(string fullName1, string fullName2)
    {
        return ConvertToResult(fullName1, string.Empty, fullName2);
    }

    /// <summary>
    /// Validates the social worker name provided against the name retrieved from the SWE API
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="fullName"></param>
    /// <returns>A normalised value [0..1]</returns>
    /// <exception cref="ArgumentException">Any input parameters passed into the method are null</exception>
    /// <exception cref="ArgumentException">If both <see cref="firstName"/> and <see cref="lastName"/> are empty or whitespace</exception>
    double Normalise(string firstName, string lastName, string fullName);

    /// <summary>
    /// Validates the social worker name provided against the name retrieved from the SWE API
    /// </summary>
    /// <seealso cref="Normalise(string,string,string)"/>
    double Normalise(string fullName1, string fullName2)
    {
        return Normalise(fullName1, string.Empty, fullName2);
    }
}
