using System.Xml;
using System.Xml.Serialization;
using Elskom.Generic.Libs;
using TMF.NET.Helpers;
using TMF.NET.Requests;
using TMF.NET.Responses;

#pragma warning disable CS8618

namespace TMF.NET;

public class GameSession
{
    private GameSession()
    {
    }

    public GameSession(string login, int sessionId, GameServer? gameServer = null)
    {
        Login = login;
        SessionId = sessionId;
        GameServer = gameServer ?? GameServer.Default;
    }

    internal GameSession(ResponseBase<OpenSessionRequest, OpenSessionResponse> openSessionResponse, string password)
    {
        Login = openSessionResponse.Request.Session.Login;
        SessionId = openSessionResponse.Response.Content.SessionId;
        GameServer = openSessionResponse.Request.Session.GameServer;

        string cr = openSessionResponse.Request.Content.Params!.Cr;
        string s = openSessionResponse.Response.Content.S;
        Blowfish = new BlowFish(CryptoHelper.MD5(cr + CryptoHelper.MD5(s + password)));
    }

    [XmlElement("login")]
    public string Login { get; set; }

    [XmlElement("session")]
    public int SessionId { get; set; }

    [XmlIgnore]
    public GameServer GameServer { get; internal set; }

    [XmlIgnore]
    internal BlowFish Blowfish { get; }

    [XmlIgnore]
    private int AuthCount { get; set; }

    internal string GenerateAuthValue<TRequest>(TRequest request) where TRequest : RequestBase<TRequest>
    {
        if (Blowfish == null) throw new Exception("This session has no password associated.");

        const string salt1 = "00000000";
        const string salt2 = "00000000";

        var xmlSerializer = GameApi.XmlSerializerCache.GetOrAdd(typeof(RequestBase<TRequest>.RequestBaseRequest),
            type => new XmlSerializer(type, new XmlRootAttribute("request")));

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, GameApi.XmlWriterSettings);

        xmlSerializer.Serialize(xmlWriter, request.Content, GameApi.XmlSerializerNamespaces);
        string xmlRequestContent = stringWriter.ToString();

        return Blowfish.EncryptCBC(
            salt1 + salt2 +
            CryptoHelper.MD5(salt2 + xmlRequestContent).ToUpper() +
            (++AuthCount).ToString("X48"))
            .ToUpper();
    }
}
