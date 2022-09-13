// This demo project will:
// - Connect to an account
// - Retrieve some info including the linked email address and owned coppers
// - Validate player key (per-session requirement for spending coppers)
// - Send a message to a player, including an optional donation

using TMF.NET;

const string Login = "19191"; // Account login
const string Password = "19191"; // Account password
const string PlayerKey = "5EC"; // Only the last 3 characters are required for spending coppers
const string Recipient = "leak"; // Login of the player to send a message to (will cost 2 coppers)
const int Donation = 0; // Amount of coppers to send along with the message (will cost an extra 5% Nadeo fee)

// First of all, instantiate a GameApi object - used to interact with... well, the game API
// If you wish, you can also pass a IWebProxy object for it to use
var gameApi = new GameApi();

// Open a new session with the account login
// You can also specify the account's game server (Nations/United) if you know it - if you don't, TMF.NET will handle it for you but then it might have to send 1 extra request
// Returns a unique ID and a salt, which is being used for hashing values (not to worry about, as TMF.NET handles all of that for you behind the scenes!)
var openSession = await gameApi.OpenSessionAsync(Login);

// Connect with our previously created session, and a password
// If the password is correct, our session ID will then become authenticated server-side, and we will be able to use this GameSession object to proceed further
var gameSession = await gameApi.ConnectAsync(openSession, Password);

Console.WriteLine($"Logged in as {Login}");
Console.WriteLine($"Session ID: {gameSession.SessionId}");
Console.WriteLine();

try
{
    // Retrieve some info about the account online profile (ranking, location, email, coppers...)
    // I have only included a few values I considered relevant, but way more data is actually available
    // If you wish, you could create your own custom response class with more values by making it implement GetOnlineProfileResponse or even just ResponseBase
    // Then, you could use it to send a GetOnlineProfileRequest and retrieve the data you're looking for, just like that:
    // await gameApi.GetResponseAsync<GetOnlineProfileRequest, MyCustomGetOnlineProfileResponse>(new GetOnlineProfileRequest(), gameSession)
    var onlineProfile = await gameApi.GetOnlineProfileAsync(gameSession);

    int coppers = onlineProfile.Response.Content.Coppers;
    int maxSpendableCoppers = (int)Math.Floor((coppers - 2) / 1.05);

    Console.WriteLine($"Email: {onlineProfile.Response.Content.Email}");
    Console.WriteLine($"Coppers: {coppers}");
    Console.WriteLine();

    if (gameSession.GameServer != GameServer.United)
        throw new("Not a United account - Message not sent.");

    if (string.IsNullOrWhiteSpace(PlayerKey))
        throw new($"{nameof(PlayerKey)} not set - Message not sent.");

    if (string.IsNullOrWhiteSpace(Recipient))
        throw new($"{nameof(Recipient)} not set - Message not sent.");

    if (Donation != 0 && maxSpendableCoppers < Donation)
        throw new("Not enough Coppers - Message not sent.");

    // Only the last 3 characters are required
    string playerKey = PlayerKey[^3..];

    // Submit, validate, associate player key with our session - required for spending coppers
    // In order to avoid this extra request, you can also pass the key to the ConnectAsync method right away
    if (!(await gameApi.ValidatePlayerKeyAsync(gameSession, playerKey)))
        throw new("Invalid player key.");

    // Send a message to a player
    // Here we are only setting a subject and donation, but we can also obviously set a message body as well
    await gameApi.SendMessageAsync(gameSession, Recipient, "Hello from TMF.NET (:", donation: Donation);

    // That's it for the demo!
    // I really hope you enjoy this library, as I did my best to make it as elegant and easy-to-use as possible
    // If you do, please make sure to Star the repo on GitHub, that would be very much appreciated!
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Done, message sent to {Recipient}");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
}
finally
{
    Console.ResetColor();

    // Do not forget to disconnect a session once you're done
    // This is required if you wish to login to the same account later, as the game might throw an "already connected" error otherwise
    await gameApi.DisconnectAsync(gameSession);
}
