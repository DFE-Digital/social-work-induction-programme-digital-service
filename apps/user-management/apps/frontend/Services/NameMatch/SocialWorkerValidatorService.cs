using System.Text;
using Dfe.Sww.Ecf.Frontend.Models.NameMatch;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.NameMatch;

/// <inheritdoc />
public class SocialWorkerValidatorService : ISocialWorkerValidatorService
{
    /// <summary>
    /// List of matchers in the order they're run
    /// </summary>
    private readonly IList<IMatcher> _matchers = new List<IMatcher>
    {
        new ExactNameMatcher(),
        new LevensteinMatcher(),
        new JaroWinklerDistanceMatcher()
    };

    /// <inheritdoc />
    public MatchResult ConvertToResult(string firstName, string lastName, string fullName)
    {
        var response = Normalise(firstName, lastName, fullName);

        var result = response * 100;

        return result switch
        {
            < 20 => MatchResult.Fail,
            < 40 => MatchResult.Poor,
            < 60 => MatchResult.Average,
            < 80 => MatchResult.Good,
            < 100 => MatchResult.VeryGood,
            100 => MatchResult.Exact,
            _ => MatchResult.Fail
        };
    }

    /// <inheritdoc />
    public double Normalise(string firstName, string lastName, string fullName)
    {
        if (firstName is null || lastName is null || fullName is null)
        {
            throw new ArgumentNullException();
        }

        if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentNullException();
        }

        var a = NormaliseString(firstName);
        var b = NormaliseString(lastName);
        var c = NormaliseString(fullName);

        return RunMatchers(a, b, c, _matchers);
    }

    private string NormaliseString(string value)
    {
        var response = value.Normalize(NormalizationForm.FormC).Trim().ToLower();

        return response;
    }

    private double RunMatchers(
        string firstName,
        string lastName,
        string sweFullName,
        IList<IMatcher> matchers
    )
    {
        var overallScore = 0.0;
        var nameConcat = $"{firstName} {lastName}".Trim();

        // Isolate Exact to run on its own
        var exactMatch = matchers[0].MatchConfidence(nameConcat, sweFullName);
        if (exactMatch > 0)
        {
            return exactMatch;
        }

        var orderedDfeName = SortNames(nameConcat);
        var orderedSweName = SortNames(sweFullName);

        // Get an average result on the rest
        for (var i = 1; i < matchers.Count; i++)
        {
            var response = matchers[i].MatchConfidence(orderedDfeName, orderedSweName);

            overallScore += response;
        }

        return overallScore / (_matchers.Count - 1);
    }

    /// <summary>
    /// Warning - Argument is expected to be normalised and lower case
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string SortNames(string name)
    {
        var splitName = name.Split(" ");
        var orderedNames = splitName.OrderBy(x => x).ToList();
        var joinedNames = string.Join(" ", orderedNames);

        return joinedNames;
    }
}
