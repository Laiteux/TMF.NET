using System.Xml.Serialization;

namespace TMF.NET.Requests;

public class ValidateSoloAccountRequest : RequestBase<ValidateSoloAccountRequest>
{
    private ValidateSoloAccountRequest()
    {
    }

    internal ValidateSoloAccountRequest(GameSession session, string last3characters) : base("ValidateSoloAccount", new()
    {
        ValidationKey = session.Blowfish.EncryptCBC(last3characters.ToUpper()).ToUpper()
    }, true)
    {
    }

    [XmlElement("vk")]
    public string ValidationKey { get; set; }
}
