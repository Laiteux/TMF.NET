using System.ComponentModel;
using System.Xml.Serialization;
using TMF.NET.Requests;

namespace TMF.NET.Responses;

public class TmfCheckLoginResponse : TmfResponseBase<TmfCheckLoginRequest, TmfCheckLoginResponse>
{
    public bool Exists => _E == 1;
    public bool IsUnited => _P == 1;
    public TmfGameServer GameServer => IsUnited ? TmfGameServer.United : TmfGameServer.Nations;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("e")]
    public int _E { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("p")] // paid/premium I guess
    public int _P { get; set; }
}
