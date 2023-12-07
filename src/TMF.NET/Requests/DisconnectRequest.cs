namespace TMF.NET.Requests;

public class DisconnectRequest : RequestBase<DisconnectRequest>
{
    public DisconnectRequest()
        : base("Disconnect", null, true)
    {
    }
}
