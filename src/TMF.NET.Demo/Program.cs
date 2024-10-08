// This demo project will:
// - Connect to a TrackMania Forever account
// - Retrieve some info like the associated email address and owned copper amount
// - Submit/validate/associate player key (per-session requirement for spending coppers)
// - Send an in-game private message to a player, including an optional copper amount

using TMF.NET;

const string Login = "YOUR_LOGIN"; // Account login
const string Password = "YOUR_PASSWORD"; // Account password
const string PlayerKey = "XXX"; // Only the last 3 characters are required
const string Recipient = "shadyx.tm"; // Login of the player to send a private message to (will cost 2 coppers)
const int Donation = 0; // Amount of coppers to send along with the message (will cost an extra 5% Nadeo fee)

// First of all, instantiate a TmfGameApi object
// You can also optionally pass a IWebProxy object as parameter
var gameApi = new TmfGameApi();

// Then open a new session associated with the account's Login
// You can also specify the account's game server (Nations/United) if you know it - if you don't, TMF.NET will figure it out but then it might have to send 1 extra request
var openSession = await gameApi.OpenSessionAsync(Login);

// Now, authenticate using our openSession and the account's Password
// If the Password is correct, our openSession will then become a gameSession, as we are able to to use the returned TmfGameSession object to proceed with further authenticated requests
var gameSession = await gameApi.ConnectAsync(openSession, Password);

Console.WriteLine($"Successfully logged in as {Login}");
Console.WriteLine();

try
{
    // Retrieve some info about the account's online profile (ranking, location, email, coppers...)
    // I have only included a few values I considered relevant, but way more data is actually available
    // If you wish, you could create your own custom response class with more values by making it implement TmfGetOnlineProfileResponse or even just TmfResponseBase
    // Then, you could use it to send a TmfGetOnlineProfileRequest and retrieve the data you're looking for, just like that:
    // await gameApi.GetResponseAsync<TmfGetOnlineProfileRequest, MyCustomGetOnlineProfileResponse>(new TmfGetOnlineProfileRequest(), gameSession)
    var onlineProfile = await gameApi.GetOnlineProfileAsync(gameSession);

    long coppers = onlineProfile.Response.Content.Coppers;
    long maxSpendableCoppers = (long)Math.Floor((coppers - 2) / 1.05);

    Console.WriteLine($"Email: {onlineProfile.Response.Content.Email}");
    Console.WriteLine($"Coppers: {coppers}");
    Console.WriteLine();

    if (gameSession.GameServer != TmfGameServer.United)
        throw new("Not a United account - Message not sent.");

    if (string.IsNullOrWhiteSpace(PlayerKey))
        throw new($"{nameof(PlayerKey)} not set - Message not sent.");

    if (string.IsNullOrWhiteSpace(Recipient))
        throw new($"{nameof(Recipient)} not set - Message not sent.");

    if (Donation != 0 && maxSpendableCoppers < Donation)
        throw new("Not enough Coppers - Message not sent.");

    // Submit/validate/associate player key with our session - this is required for spending coppers
    // In order to avoid this extra request, you could also pass the key directly as a parameter to the ConnectAsync method
    if (!await gameApi.ValidatePlayerKeyAsync(gameSession, PlayerKey))
        throw new("Invalid player key.");

    // Send a message to a player
    // Here we are only setting a subject and donation, but we could obviously set a message body (text) as well
    await gameApi.SendMessageAsync(gameSession, Recipient, "Hello from TMF.NET (:", null, Donation);

    // That's it for the demo!
    // If you enjoy this library then please consider Starring the repo on GitHub, that would be very much appreciated (:
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Successfully sent message to {Recipient}");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
}
finally
{
    // Do not forget to disconnect a session once you are done with it
    // This is required if you wish to login to the same account later, as the game/API might throw an "already connected" error otherwise
    await gameApi.DisconnectAsync(gameSession);
}

Console.ReadKey(true);
