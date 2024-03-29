﻿using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using TMF.NET.Requests;

#pragma warning disable CS8618

namespace TMF.NET.Responses;

public class TmfResponseBase<TRequest, TResponse>
    where TRequest : TmfRequestBase<TRequest>
    where TResponse : TmfResponseBase<TRequest, TResponse>
{
    [XmlIgnore]
    public TRequest Request { get; set; }

    [XmlElement("r")]
    public ResponseBaseResponse Response { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlElement("e")]
    public string ExecutionTimeString { get; set; }

    public TimeSpan ExecutionTime
        => TimeSpan.FromSeconds(double.Parse(ExecutionTimeString.Split(' ')[3], CultureInfo.InvariantCulture));

    public class ResponseBaseResponse
    {
        [XmlElement("n")]
        public string ProcedureName { get; set; }

        [XmlElement("c")]
        public TResponse Content { get; set; }

        [XmlElement("e")]
        public ResponseBaseResponseError? Error { get; set; }

        public class ResponseBaseResponseError
        {
            [XmlElement("v")]
            public int Code { get; set; }

            [XmlElement("m")]
            public string? Message { get; set; }

            public void Throw()
                => throw new TmfGameApiException(Code, Message);
        }
    }
}
