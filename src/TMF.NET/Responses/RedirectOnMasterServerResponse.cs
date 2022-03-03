using System.ComponentModel;
using System.Xml.Serialization;
using TMF.NET.Requests;

namespace TMF.NET.Responses;

public class RedirectOnMasterServerResponse : ResponseBase<NoRequest, RedirectOnMasterServerResponse>
{
    public string NewDomain => _A.NewDomain;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("a")]
    public RedirectOnMasterServerResponseA _A { get; set; }

    public class RedirectOnMasterServerResponseA
    {
        [XmlElement("c")]
        public string NewDomain { get; set; }
    }
}
