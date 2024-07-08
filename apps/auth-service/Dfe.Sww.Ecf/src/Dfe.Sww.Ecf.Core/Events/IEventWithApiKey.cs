using Dfe.Sww.Ecf.Core.Events.Models;

namespace Dfe.Sww.Ecf.Core.Events;

public interface IEventWithApiKey
{
    ApiKey ApiKey { get; }
}
