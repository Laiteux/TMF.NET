﻿using System.Xml.Serialization;

namespace TMF.NET.Requests;

public class OpenSessionRequest : RequestBase<OpenSessionRequest>
{
    private OpenSessionRequest(string cr)
    {
        Cr = cr;
    }

    public OpenSessionRequest() : base("OpenSession", new OpenSessionRequest("00000000"))
    {
    }

    [XmlElement("cr")]
    public string Cr { get; set; }
}
