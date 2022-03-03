// This demo project will:
// - Connect to an account
// - Retrieve email & coppers
// - Validate player key
// - Send a message to a player

using TMF.NET;

const string Login = "19191"; // Account login
const string Password = "19191"; // Account password
const string PlayerKey = "5EC"; // Only the last 3 characters are required for spending coppers
const string Recipient = "nadeo"; // Login of player to send a test message to (will cost 2 coppers)
const int Donation = 0; // Amount of coppers to send to recipient along with the message

// Instantiate a GameApi object - used to interact with... well, the Game API
// You can also give it a proxy to use if you wish
var gameApi = new GameApi();

// Open a new session with the account login
// You can also specify the account's game server (Nations/United) - if you don't, TMF.NET will handle that for you but then it might have to send 1 extra request
// Returns a unique ID and a salt, which is then being used for hashing values (TMF.NET handles all of that for you!)
var openSession = await gameApi.OpenSessionAsync(Login);

// Connect with our previously created session and a password
// If the password is correct, our session ID will then become authenticated
var gameSession = await gameApi.ConnectAsync(openSession, Password);

Console.WriteLine($"Logged in with {Login}");
Console.WriteLine($"Session ID: {gameSession.SessionId}");
Console.WriteLine();

try
{
    // Retrieve some info about the account online profile (ranking, location, email, coppers...)
    // I have only included a few values I considered relevant, but way more data is actually available
    // You can create your own response class with more values and make it implement GetOnlineProfileResponse or even just ResponseBase
    // Then you can get a response like that: gameApi.GetResponseAsync<GetOnlineProfileRequest, MyCustomGetOnlineProfileResponse>(new(), gameSession)
    var onlineProfile = await gameApi.GetOnlineProfileAsync(gameSession);

    Console.WriteLine($"Email: {onlineProfile.Response.Content.Email}");
    Console.WriteLine($"Coppers: {onlineProfile.Response.Content.Coppers}");
    Console.WriteLine();

    if (gameSession.GameServer != GameServer.United)
        throw new("Not a United account - Message not sent.");

    if (string.IsNullOrWhiteSpace(PlayerKey))
        throw new($"{nameof(PlayerKey)} not set - Message not sent.");

    if (string.IsNullOrWhiteSpace(Recipient))
        throw new($"{nameof(Recipient)} not set - Message not sent.");

    if (onlineProfile.Response.Content.Coppers < 2)
        throw new("Not enough Coppers - Message not sent.");

    // Keep only the last 3 characters
    string playerKey = PlayerKey[^3..];

    // Submit, validate, associate player key with our session - required for spending coppers
    // In order to avoid this extra request, you can also pass the key to the ConnectAsync method right away
    await gameApi.ValidatePlayerKeyAsync(gameSession, playerKey);

    // Send a message to a player
    // Here we are only setting a subject and donation, but we can also obviously set a message body as well
    // Note that if you're sending a donation, Nadeo will charge you 5% + 2 Coppers
    // Simple way to deduct Nadeo fee from amount: (int)Math.Floor((Donation - 2) / 1.05)
    await gameApi.SendMessageAsync(gameSession, Recipient, "Hello from TMF.NET (:", donation: Donation);

    // That's it for the demo!
    // I really hope you like this library, I did my best to make it as elegant and easy-to-use as possible
    // If you do, please make sure to Star the repo on GitHub, as that would be very much appreciated!
    Console.WriteLine($"Message sent to {Recipient}");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
}
finally
{
    // Do not forget to disconnect a session once you're done
    // This is required if you wish to login to the same account later, as the game might throw an "already connected" error otherwise
    await gameApi.DisconnectAsync(gameSession);
}
