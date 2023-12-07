using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using TMF.NET.Extensions;
using TMF.NET.Requests;
using TMF.NET.Responses;

namespace TMF.NET;

public class GameApi
{
    internal static readonly ConcurrentDictionary<Type, XmlSerializer> XmlSerializerCache = new();
    internal static readonly XmlWriterSettings XmlWriterSettings = new() { Async = true, OmitXmlDeclaration = true };
    internal static readonly XmlSerializerNamespaces XmlSerializerNamespaces = new(new XmlQualifiedName[] { new("", "") });

    private readonly HttpClient _httpClient;

    public GameApi() : this(null!)
    {
    }

    public GameApi(IWebProxy proxy, bool rotating = false, TimeSpan? timeout = null)
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

    public async Task<ResponseBase<TRequest, TResponse>> GetResponseAsync<TRequest, TResponse>(TRequest request, GameSession? session = null)
        where TRequest : RequestBase<TRequest>
        where TResponse : ResponseBase<TRequest, TResponse>
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

        string apiUrl = $"http://{(request.OverrideGameServer ?? session?.GameServer) ?? GameServer.Default}/online_game/request.php";

        using var responseMessage = await _httpClient.PostAsync(apiUrl, new StringContent(xmlString));
        var responseContentString = await responseMessage.Content.ReadAsStringAsync();

        // Some responses such as Connect contain a bunch of weird characters after their XML content, so we trim them
        responseContentString = new Regex(@"(</\w+>)(?!<).+", RegexOptions.Singleline | RegexOptions.Compiled)
            .Replace(responseContentString, m => m.Groups[1].Value);

        await using var responseContentStream = responseContentString.ToStream();

        xmlSerializer = XmlSerializerCache.GetOrAdd(typeof(ResponseBase<TRequest, TResponse>),
            type => new XmlSerializer(type, new XmlRootAttribute("r")));

        var response = (ResponseBase<TRequest, TResponse>)xmlSerializer.Deserialize(responseContentStream)!;

        if (response.Response.ProcedureName == "RedirectOnMasterServer")
        {
            xmlSerializer = XmlSerializerCache.GetOrAdd(typeof(ResponseBase<NoRequest, RedirectOnMasterServerResponse>),
                type => new XmlSerializer(type, new XmlRootAttribute("r")));

            responseContentStream.Position = 0;
            var redirectResponse = (ResponseBase<NoRequest, RedirectOnMasterServerResponse>)xmlSerializer.Deserialize(responseContentStream)!;

            session!.GameServer = redirectResponse.Response.Content.NewDomain;

            // ReSharper disable once TailRecursiveCall
            return await GetResponseAsync<TRequest, TResponse>(request, session);
        }

        response.Request = request;

        response.Response.Error?.Throw();

        return response;
    }

    public async Task<ResponseBase<OpenSessionRequest, OpenSessionResponse>> OpenSessionAsync(
        string login,
        GameServer? gameServer = null)
    {
        return await GetResponseAsync<OpenSessionRequest, OpenSessionResponse>(
            new OpenSessionRequest(),
            new GameSession(login, -1, gameServer));
    }

    public async Task<GameSession> ConnectAsync(
        ResponseBase<OpenSessionRequest, OpenSessionResponse> openSessionResponse,
        string password,
        string? playerKeyLast3characters = null)
    {
        var session = new GameSession(openSessionResponse, password);

        await GetResponseAsync<ConnectRequest, ConnectResponse>(
            new ConnectRequest(session, playerKeyLast3characters),
            session);

        return session;
    }

    public async Task<ResponseBase<DisconnectRequest, DisconnectResponse>> DisconnectAsync(
        GameSession session)
    {
        return await GetResponseAsync<DisconnectRequest, DisconnectResponse>(
            new DisconnectRequest(),
            session);
    }

    public async Task<ResponseBase<GetOnlineProfileRequest, GetOnlineProfileResponse>> GetOnlineProfileAsync(
        GameSession session)
    {
        return await GetResponseAsync<GetOnlineProfileRequest, GetOnlineProfileResponse>(
            new GetOnlineProfileRequest(),
            session);
    }

    public async Task<ResponseBase<CheckLoginRequest, CheckLoginResponse>> CheckLoginAsync(
        string login)
    {
        return await GetResponseAsync<CheckLoginRequest, CheckLoginResponse>(
            new CheckLoginRequest(login));
    }

    public async Task<ResponseBase<SendMessagesRequest, SendMessagesResponse>> SendMessageAsync(
        GameSession session,
        string recipient,
        string? subject,
        string? message,
        long donation = 0)
    {
        return await GetResponseAsync<SendMessagesRequest, SendMessagesResponse>(
            new SendMessagesRequest(recipient, subject, message, donation),
            session);
    }

    public async Task<bool> ValidatePlayerKeyAsync(
        GameSession session,
        string last3characters)
    {
        try
        {
            await GetResponseAsync<ValidateSoloAccountRequest, ValidateSoloAccountResponse>(
                new ValidateSoloAccountRequest(session, last3characters),
                session);

            return true;
        }
        catch (Exception ex)
        {
            if (ex is GameApiException { ErrorCode: GameApiException.InvalidPlayerKey })
            {
                return false;
            }

            throw;
        }
    }
}
