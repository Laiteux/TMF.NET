using System.Xml.Serialization;

namespace TMF.NET.Requests;

public class SendMessagesRequest : RequestBase<SendMessagesRequest>
{
    private SendMessagesRequest()
    {
    }

    internal SendMessagesRequest(string recipient, string subject, string message, int donation) : base("SendMessages", new()
    {
        Recipient = recipient,
        Subject = subject ?? " ",
        Message = message ?? "+",
        Donation = donation.ToString() // can't use int because it's buggy
    }, true)
    {
    }

    [XmlElement("r1")]
    public string Recipient { get; set; }

    [XmlElement("s1")]
    public string Subject { get; set; }

    [XmlElement("m1")]
    public string Message { get; set; }

    [XmlElement("d1")]
    public string Donation { get; set; }
}
