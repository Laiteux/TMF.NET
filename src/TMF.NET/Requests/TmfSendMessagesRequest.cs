using System.Xml.Serialization;

#pragma warning disable CS8618

namespace TMF.NET.Requests;

public class TmfSendMessagesRequest : TmfRequestBase<TmfSendMessagesRequest>
{
    private TmfSendMessagesRequest()
    {
    }

    public TmfSendMessagesRequest(string recipient, string? subject, string? message, long donation)
        : base("SendMessages", new()
        {
            Recipient = recipient,
            Subject = subject ?? " ",
            Message = message ?? "+",
            Donation = donation.ToString() // can't use long because it's buggy
        }, true)
    {
    }

    [XmlElement("r1")]
    public string Recipient { get; /*private*/ set; }

    [XmlElement("s1")]
    public string Subject { get; /*private*/ set; }

    [XmlElement("m1")]
    public string Message { get; /*private*/ set; }

    [XmlElement("d1")]
    public string Donation { get; /*private*/ set; }
}
