using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using TMF.NET.Extensions;
using TMF.NET.Requests;
using TMF.NET.Responses;

namespace TMF.NET;

public class TmfGameApi
{
    internal static readonly ConcurrentDictionary<Type, XmlSerializer> XmlSerializerCache = new();
    internal static readonly XmlWriterSettings XmlWriterSettings = new() { Async = true, OmitXmlDeclaration = true };
    internal static readonly XmlSerializerNamespaces XmlSerializerNamespaces = new(new XmlQualifiedName[] { new("", "") });

    private readonly HttpClient _httpClient;

    public TmfGameApi() : this(null!)
    {
    }

    public TmfGameApi(IWebProxy proxy, bool rotating = false, TimeSpan? timeout = null)
    {
        _httpClient = new(new HttpClientHandler()
        {
            UseCookies = false,
            Proxy = proxy
        })
        {
            Timeout = timeout ?? TimeSpan.FromSeconds(10)
        };

        _httpClient.DefaultRequestHeaders.ConnectionClose = rotating;
    }

    public async Task<TmfResponseBase<TRequest, TResponse>> GetResponseAsync<TRequest, TResponse>(TRequest request, TmfGameSession? session = null)
        where TRequest : TmfRequestBase<TRequest>
        where TResponse : TmfResponseBase<TRequest, TResponse>
    {
        if (session != null)
            request.Session = session;

        if (request.RequiresAuth)
            request.Auth = new() { Value = session!.GenerateAuthValue(request) };

        var xmlSerializer = XmlSerializerCache.GetOrAdd(typeof(TRequest),
            type => new XmlSerializer(type, new XmlRootAttribute("root")));

        await using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, XmlWriterSettings);

        xmlSerializer.Serialize(xmlWriter, request, XmlSerializerNamespaces);
        string xmlString = stringWriter.ToString();

        string apiUrl = $"http://{(request.OverrideGameServer ?? session?.GameServer) ?? TmfGameServer.Default}/online_game/request.php";

        using var responseMessage = await _httpClient.PostAsync(apiUrl, new StringContent(xmlString));
        var responseContentString = await responseMessage.Content.ReadAsStringAsync();

        // Some responses such as Connect contain a bunch of weird characters after their XML content, so we trim them
        responseContentString = new Regex(@"(</\w+>)(?!<).+", RegexOptions.Singleline | RegexOptions.Compiled)
            .Replace(responseContentString, m => m.Groups[1].Value);

        await using var responseContentStream = responseContentString.ToStream();

        xmlSerializer = XmlSerializerCache.GetOrAdd(typeof(TmfResponseBase<TRequest, TResponse>),
            type => new XmlSerializer(type, new XmlRootAttribute("r")));

        var response = (TmfResponseBase<TRequest, TResponse>)xmlSerializer.Deserialize(responseContentStream)!;

        if (response.Response.ProcedureName == "RedirectOnMasterServer")
        {
            xmlSerializer = XmlSerializerCache.GetOrAdd(typeof(TmfResponseBase<TmfNoRequest, TmfRedirectOnMasterServerResponse>),
                type => new XmlSerializer(type, new XmlRootAttribute("r")));

            responseContentStream.Position = 0;
            var redirectResponse = (TmfResponseBase<TmfNoRequest, TmfRedirectOnMasterServerResponse>)xmlSerializer.Deserialize(responseContentStream)!;

            session!.GameServer = redirectResponse.Response.Content.NewDomain;

            // ReSharper disable once TailRecursiveCall
            return await GetResponseAsync<TRequest, TResponse>(request, session);
        }

        response.Request = request;

        response.Response.Error?.Throw();

        return response;
    }

    public async Task<TmfResponseBase<TmfOpenSessionRequest, TmfOpenSessionResponse>> OpenSessionAsync(
        string login,
        TmfGameServer? gameServer = null)
    {
        return await GetResponseAsync<TmfOpenSessionRequest, TmfOpenSessionResponse>(
            new TmfOpenSessionRequest(),
            new TmfGameSession(login, -1, gameServer));
    }

    public async Task<TmfGameSession> ConnectAsync(
        TmfResponseBase<TmfOpenSessionRequest, TmfOpenSessionResponse> openSessionResponse,
        string password,
        string? playerKeyLast3characters = null)
    {
        var session = new TmfGameSession(openSessionResponse, password);

        await GetResponseAsync<TmfConnectRequest, TmfConnectResponse>(
            new TmfConnectRequest(session, playerKeyLast3characters),
            session);

        return session;
    }

    public async Task<TmfResponseBase<TmfDisconnectRequest, TmfDisconnectResponse>> DisconnectAsync(
        TmfGameSession session)
    {
        return await GetResponseAsync<TmfDisconnectRequest, TmfDisconnectResponse>(
            new TmfDisconnectRequest(),
            session);
    }

    public async Task<TmfResponseBase<TmfGetOnlineProfileRequest, TmfGetOnlineProfileResponse>> GetOnlineProfileAsync(
        TmfGameSession session)
    {
        return await GetResponseAsync<TmfGetOnlineProfileRequest, TmfGetOnlineProfileResponse>(
            new TmfGetOnlineProfileRequest(),
            session);
    }

    public async Task<TmfResponseBase<TmfCheckLoginRequest, TmfCheckLoginResponse>> CheckLoginAsync(
        string login)
    {
        return await GetResponseAsync<TmfCheckLoginRequest, TmfCheckLoginResponse>(
            new TmfCheckLoginRequest(login));
    }

    public async Task<TmfResponseBase<TmfSendMessagesRequest, TmfSendMessagesResponse>> SendMessageAsync(
        TmfGameSession session,
        string recipient,
        string? subject = null,
        string? message = null,
        long donation = 0)
    {
        return await GetResponseAsync<TmfSendMessagesRequest, TmfSendMessagesResponse>(
            new TmfSendMessagesRequest(recipient, subject, message, donation),
            session);
    }

    public async Task<bool> ValidatePlayerKeyAsync(
        TmfGameSession session,
        string last3characters)
    {
        try
        {
            await GetResponseAsync<TmfValidateSoloAccountRequest, TmfValidateSoloAccountResponse>(
                new TmfValidateSoloAccountRequest(session, last3characters),
                session);

            return true;
        }
        catch (Exception ex)
        {
            if (ex is TmfGameApiException { ErrorCode: TmfGameApiException.InvalidPlayerKey })
            {
                return false;
            }

            throw;
        }
    }
}
