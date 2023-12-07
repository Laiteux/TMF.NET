using System.Xml.Serialization;

#pragma warning disable CS8618

namespace TMF.NET.Requests;

public class TmfOpenSessionRequest : TmfRequestBase<TmfOpenSessionRequest>
{
    private TmfOpenSessionRequest(string cr)
    {
        Cr = cr;
    }

    public TmfOpenSessionRequest()
        : base("OpenSession", new TmfOpenSessionRequest("00000000"))
    {
    }

    [XmlElement("cr")]
    public string Cr { get; /*private*/ set; }
}
