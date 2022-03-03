using System.Xml.Serialization;

namespace TMF.NET.Requests;

public class CheckLoginRequest : RequestBase<CheckLoginRequest>
{
    private CheckLoginRequest()
    {
    }

    internal CheckLoginRequest(string login) : base("CheckLogin", new()
    {
        Login = login
    })
    {
    }

    [XmlElement("l")]
    public string Login { get; set; }
}
