using System.Xml.Serialization;

#pragma warning disable CS8618

namespace TMF.NET.Requests;

public class CheckLoginRequest : RequestBase<CheckLoginRequest>
{
    private CheckLoginRequest()
    {
    }

    internal CheckLoginRequest(string login)
        : base("CheckLogin", new()
        {
            Login = login
        }, overrideGameServer: GameServer.United)
    {
    }

    [XmlElement("l")]
    public string Login { get; set; }
}
