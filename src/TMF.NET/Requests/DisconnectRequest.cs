namespace TMF.NET.Requests;

public class DisconnectRequest : RequestBase<DisconnectRequest>
{
    internal DisconnectRequest() : base("Disconnect", null, true)
    {
    }
}
