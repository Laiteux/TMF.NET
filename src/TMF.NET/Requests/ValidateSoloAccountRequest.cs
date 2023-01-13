using System.Xml.Serialization;

#pragma warning disable CS8618

namespace TMF.NET.Requests;

public class ValidateSoloAccountRequest : RequestBase<ValidateSoloAccountRequest>
{
    private ValidateSoloAccountRequest()
    {
    }

    internal ValidateSoloAccountRequest(GameSession session, string last3characters) : base("ValidateSoloAccount", new()
    {
        ValidationKey = session.Blowfish.EncryptCBC(last3characters[^3..].ToUpper()).ToUpper()
    }, true)
    {
    }

    [XmlElement("vk")]
    public string ValidationKey { get; set; }
}
