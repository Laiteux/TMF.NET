﻿using System.ComponentModel;
using System.Xml.Serialization;
using TMF.NET.Requests;

#pragma warning disable CS8618

namespace TMF.NET.Responses;

public class GetOnlineProfileResponse : ResponseBase<GetOnlineProfileRequest, GetOnlineProfileResponse>
{
    public long OnlineRanking => _B.OnlineRanking;
    public string Location => _D.Location;
    public string Email => _D.Email;
    public string HtmlNickname => _D.HtmlNickname;
    public long Coppers => _F.Coppers;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("b")]
    public GetOnlineProfileResponseB _B { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("d")]
    public GetOnlineProfileResponseD _D { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("f")]
    public GetOnlineProfileResponseF _F { get; set; }

    public class GetOnlineProfileResponseB
    {
        [XmlElement("g")]
        public long OnlineRanking { get; set; }
    }

    public class GetOnlineProfileResponseD
    {
        [XmlElement("p")]
        public string Location { get; set; }

        [XmlElement("m")]
        public string Email { get; set; }

        [XmlElement("n")]
        public string HtmlNickname { get; set; }
    }

    public class GetOnlineProfileResponseF
    {
        [XmlElement("c")]
        public long Coppers { get; set; }
    }
}
