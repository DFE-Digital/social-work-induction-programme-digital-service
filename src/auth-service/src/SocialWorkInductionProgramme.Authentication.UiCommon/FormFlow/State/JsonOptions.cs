using System.Text.Json;

namespace SocialWorkInductionProgramme.Authentication.UiCommon.FormFlow.State;

public class JsonOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions()
    {
        WriteIndented = false
    };
}
