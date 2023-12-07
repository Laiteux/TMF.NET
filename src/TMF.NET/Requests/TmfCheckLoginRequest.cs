using System.Xml.Serialization;

#pragma warning disable CS8618

namespace TMF.NET.Requests;

public class TmfCheckLoginRequest : TmfRequestBase<TmfCheckLoginRequest>
{
    private TmfCheckLoginRequest()
    {
    }

    public TmfCheckLoginRequest(string login)
        : base("CheckLogin", new()
        {
            Login = login
        }, overrideGameServer: TmfGameServer.United)
    {
    }

    [XmlElement("l")]
    public string Login { get; /*private*/ set; }
}
