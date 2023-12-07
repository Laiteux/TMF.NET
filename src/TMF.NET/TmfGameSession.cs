using System.Xml;
using System.Xml.Serialization;
using Elskom.Generic.Libs;
using TMF.NET.Helpers;
using TMF.NET.Requests;
using TMF.NET.Responses;

#pragma warning disable CS8618

namespace TMF.NET;

public class TmfGameSession
{
    private TmfGameSession()
    {
    }

    public TmfGameSession(string login, long sessionId, TmfGameServer? gameServer = null)
    {
        Login = login;
        SessionId = sessionId;
        GameServer = gameServer ?? TmfGameServer.Default;
    }

    internal TmfGameSession(TmfResponseBase<TmfOpenSessionRequest, TmfOpenSessionResponse> openSessionResponse, string password)
    {
        Login = openSessionResponse.Request.Session.Login;
        SessionId = openSessionResponse.Response.Content.SessionId;
        GameServer = openSessionResponse.Request.Session.GameServer;

        string cr = openSessionResponse.Request.Content.Params!.Cr;
        string s = openSessionResponse.Response.Content.S;
        Blowfish = new BlowFish(CryptoHelper.MD5(cr + CryptoHelper.MD5(s + password)));
    }

    [XmlElement("login")]
    public string Login { get; /*internal*/ set; }

    [XmlElement("session")]
    public long SessionId { get; /*internal*/ set; }

    [XmlIgnore]
    public TmfGameServer GameServer { get; /*internal*/ set; }

    [XmlIgnore]
    internal BlowFish Blowfish { get; }

    [XmlIgnore]
    private long AuthCount { get; set; }

    internal string GenerateAuthValue<TRequest>(TRequest request)
        where TRequest : TmfRequestBase<TRequest>
    {
        if (Blowfish == null)
            throw new InvalidOperationException("This session has no associated password.");

        const string salt1 = "00000000";
        const string salt2 = "00000000";

        var xmlSerializer = TmfGameApi.XmlSerializerCache.GetOrAdd(typeof(TmfRequestBase<TRequest>.RequestBaseRequest),
            type => new XmlSerializer(type, new XmlRootAttribute("request")));

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, TmfGameApi.XmlWriterSettings);

        xmlSerializer.Serialize(xmlWriter, request.Content, TmfGameApi.XmlSerializerNamespaces);
        var requestContentString = stringWriter.ToString();

        return Blowfish.EncryptCBC(
                salt1 + salt2 +
                CryptoHelper.MD5(salt2 + requestContentString).ToUpper() +
                (++AuthCount).ToString("X48"))
            .ToUpper();
    }
}
