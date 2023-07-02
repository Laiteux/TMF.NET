﻿using System.Xml.Serialization;

#pragma warning disable CS8618

namespace TMF.NET.Requests;

public class RequestBase<TParams> where TParams : RequestBase<TParams>
{
    protected RequestBase()
    {
    }

    protected RequestBase(string procedureName, TParams? parameters, bool requiresAuth = false, GameServer? overrideGameServer = null)
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
    public RequestBaseGame Game { get; set; }

    [XmlElement("author")]
    public GameSession Session { get; set; }

    [XmlElement("request")]
    public RequestBaseRequest Content { get; set; }

    [XmlIgnore]
    internal bool RequiresAuth { get; }

    [XmlIgnore]
    internal GameServer? OverrideGameServer { get; }

    [XmlElement("auth")]
    public RequestBaseAuth Auth { get; set; }

    public class RequestBaseGame
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("distro")]
        public string Distro { get; set; }

        [XmlElement("lang")]
        public string Lang { get; set; }

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
        public string ProcedureName { get; set; }

        [XmlElement("params")]
        public TParams? Params { get; set; }
    }

    public class RequestBaseAuth
    {
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
