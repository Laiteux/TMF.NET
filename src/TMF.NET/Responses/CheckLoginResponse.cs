using System.Xml.Serialization;
using TMF.NET.Requests;

namespace TMF.NET.Responses;

public class CheckLoginResponse : ResponseBase<CheckLoginRequest, CheckLoginResponse>
{
    [XmlElement("e")]
    public int Exists { get; set; }

    [XmlElement("p")] // paid/premium?
    public int United { get; set; }

    public GameServer GetGameServer() => United == 1 ? GameServer.United : GameServer.Nations;
}
