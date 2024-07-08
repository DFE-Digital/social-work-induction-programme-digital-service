using System.Text.Json;

namespace Dfe.Sww.Ecf.UiCommon.FormFlow.State;

public class JsonOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions()
    {
        WriteIndented = false
    };
}
