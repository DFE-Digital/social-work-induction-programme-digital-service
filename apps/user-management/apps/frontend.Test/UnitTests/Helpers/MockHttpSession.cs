using System.Text;
using Microsoft.AspNetCore.Http;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;

public class MockHttpSession : ISession
{
    private readonly Dictionary<string, object> _sessionStorage = new();

    string ISession.Id => throw new NotImplementedException();

    bool ISession.IsAvailable => throw new NotImplementedException();

    IEnumerable<string> ISession.Keys => _sessionStorage.Keys;

    void ISession.Clear()
    {
        _sessionStorage.Clear();
    }

    Task ISession.CommitAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task ISession.LoadAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    void ISession.Remove(string key)
    {
        _sessionStorage.Remove(key);
    }

    void ISession.Set(string key, byte[] value)
    {
        _sessionStorage[key] = Encoding.UTF8.GetString(value);
    }

    bool ISession.TryGetValue(string key, out byte[] value)
    {
        if (_sessionStorage.TryGetValue(key, out var keyValue))
        {
            value = Encoding.ASCII.GetBytes(keyValue.ToString() ?? string.Empty);
            return true;
        }

        value = null!;
        return false;
    }
}
