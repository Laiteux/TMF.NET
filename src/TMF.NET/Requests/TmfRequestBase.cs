using System.Xml.Serialization;

#pragma warning disable CS8618

namespace TMF.NET.Requests;

public class TmfRequestBase<TParams>
    where TParams : TmfRequestBase<TParams>
{
    protected TmfRequestBase()
    {
    }

    public TmfRequestBase(string procedureName, TParams? parameters, bool requiresAuth = false, TmfGameServer? overrideGameServer = null)
    {
        Game = RequestBaseGame.Default;

        Content = new RequestBaseRequest()
        {
            ProcedureName = procedureName,
            Params = parameters
        };

        RequiresAuth = requiresAuth;
        OverrideGameServer = overrideGameServer;
    }

    [XmlElement("game")]
    public RequestBaseGame Game { get; /*private*/ set; }

    [XmlElement("author")]
    public TmfGameSession Session { get; /*internal*/ set; }

    [XmlElement("request")]
    public RequestBaseRequest Content { get; /*private*/ set; }

    [XmlElement("auth")]
    public RequestBaseAuth Auth { get; /*internal*/ set; }

    [XmlIgnore]
    internal bool RequiresAuth { get; }

    [XmlIgnore]
    internal TmfGameServer? OverrideGameServer { get; }

    public class RequestBaseGame
    {
        [XmlElement("name")]
        public string Name { get; /*internal*/ set; }

        [XmlElement("version")]
        public string Version { get; /*internal*/ set; }

        [XmlElement("distro")]
        public string Distro { get; /*internal*/ set; }

        [XmlElement("lang")]
        public string Lang { get; /*internal*/ set; }

        internal static readonly RequestBaseGame Default = new()
        {
            Name = "TmForever",
            Version = "2.11.26",
            Distro = "ASPUR",
            Lang = "en"
        };
    }

    public class RequestBaseRequest
    {
        [XmlElement("name")]
        public string ProcedureName { get; /*internal*/ set; }

        [XmlElement("params")]
        public TParams? Params { get; /*internal*/ set; }
    }

    public class RequestBaseAuth
    {
        [XmlElement("value")]
        public string Value { get; /*internal*/ set; }
    }
}
