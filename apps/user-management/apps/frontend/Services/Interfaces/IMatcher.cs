namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

internal interface IMatcher
{
    /// <summary>
    /// Compares how close the input parameter <see cref="a"/> matches <see cref="b"/>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>A figure representing how closely the strings matched; 0 is low, 1 is high</returns>
    double MatchConfidence(string a, string b);
}
