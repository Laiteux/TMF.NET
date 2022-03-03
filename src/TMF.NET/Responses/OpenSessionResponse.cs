using System.Xml.Serialization;
using TMF.NET.Requests;

namespace TMF.NET.Responses;

public class OpenSessionResponse : ResponseBase<OpenSessionRequest, OpenSessionResponse>
{
    [XmlElement("i")]
    public int SessionId { get; set; }

    [XmlElement("s")]
    public string S { get; set; }
}
