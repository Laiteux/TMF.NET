using System.ComponentModel;
using System.Xml.Serialization;
using TMF.NET.Requests;

namespace TMF.NET.Responses;

public class CheckLoginResponse : ResponseBase<CheckLoginRequest, CheckLoginResponse>
{
    public bool Exists => _E == 1;
    public bool IsUnited => _P == 1;
    public GameServer GameServer => IsUnited ? GameServer.United : GameServer.Nations;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("e")]
    public int _E { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("p")] // paid/premium I guess
    public int _P { get; set; }
}
