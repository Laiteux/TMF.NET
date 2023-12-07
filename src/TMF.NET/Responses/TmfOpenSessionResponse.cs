using System.Xml.Serialization;
using TMF.NET.Requests;

#pragma warning disable CS8618

namespace TMF.NET.Responses;

public class TmfOpenSessionResponse : TmfResponseBase<TmfOpenSessionRequest, TmfOpenSessionResponse>
{
    [XmlElement("i")]
    public long SessionId { get; set; }

    [XmlElement("s")]
    public string S { get; set; }
}
