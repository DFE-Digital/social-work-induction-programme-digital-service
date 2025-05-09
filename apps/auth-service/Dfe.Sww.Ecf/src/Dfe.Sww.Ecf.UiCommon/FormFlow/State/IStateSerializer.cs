namespace Dfe.Sww.Ecf.UiCommon.FormFlow.State;

public interface IStateSerializer
{
    object Deserialize(Type type, string serialized);
    string Serialize(Type type, object state);
}
