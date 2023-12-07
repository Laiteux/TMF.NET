namespace TMF.NET.Requests;

public class TmfDisconnectRequest : TmfRequestBase<TmfDisconnectRequest>
{
    public TmfDisconnectRequest()
        : base("Disconnect", null, true)
    {
    }
}
