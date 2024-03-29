﻿namespace TMF.NET;

public class TmfGameApiException : Exception
{
    internal TmfGameApiException(int errorCode, string? errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public int ErrorCode { get; }

    private string? ErrorMessage { get; }

    public override string Message
        => _errors.TryGetValue(ErrorCode, out string? error) ? error : (ErrorMessage ?? $"Error code: {ErrorCode}");

    #region Implicit error values
    public const int
        Other = 1,
        InvalidLogin = 7,
        UnknownLogin = 13,
        InvalidSession = 18,
        InvalidPlayerKey = 91,
        BadPackmask = 103,
        RateLimit = 106,
        WrongPassword = 121,
        AlreadyConnected = 122;
    #endregion

    private static readonly Dictionary<int, string> _errors = new()
    {
        { 4, "game unknown" },
        { 5, "invalid version" },
        { 6, "invalid language" },
        { 7, "invalid login" },
        { 8, "invalid password" },
        { 9, "invalid key" },
        { 10, "invalid nickname" },
        { 11, "invalid new password" },
        { 12, "invalid mail" },
        { 13, "Login unknown: there is no account with this login. Please make sure you spelled your login right" },
        { 14, "player unknown" },
        { 15, "Wrong password. Please make sure you spelled your password right" },
        { 16, "Wrong key. Please make sure you spelled your key right" },
        { 17, "Someone has already created an account with this login. Please try to find another login" },
        { 18, "You are not connected" },
        { 19, "You are already connected (from a different ip) - Please try again in a moment" },
        { 20, "You will be redirected to a new master server" },
        { 21, "no master server" },
        { 22, "There is no mail in the account" },
        { 23, "The server is overloaded, please try again later" },
        { 24, "challenge unknown" },
        { 25, "challenge record unknown" },
        { 26, "invalid challenge" },
        { 27, "invalid action" },
        { 28, "invalid files" },
        { 29, "invalid code" },
        { 30, "invalid upload rate" },
        { 31, "invalid download rate" },
        { 33, "You cannot be your own buddy" },
        { 34, "This person is already your buddy" },
        { 35, "Buddy not found" },
        { 36, "self abusing" },
        { 37, "cannot open file" },
        { 38, "cannot write into file" },
        { 39, "cannot delete file" },
        { 40, "Bad zone" },
        { 41, "zone not found" },
        { 42, "team unknown" },
        { 43, "duplicate teams" },
        { 44, "A challenge with the same name already exists" },
        { 45, "There are no official challenges" },
        { 46, "cannot read file" },
        { 47, "Cannot share given challenge" },
        { 48, "invalid date" },
        { 49, "invalid record" },
        { 50, "unknown group" },
        { 51, "not in group" },
        { 52, "already subscribed" },
        { 53, "challenge attempt unknown" },
        { 54, "invalid author" },
        { 55, "author unknown" },
        { 56, "invalid challenge name" },
        { 57, "invalid environment" },
        { 58, "invalid ping" },
        { 59, "bad record" },
        { 60, "news unknown" },
        { 61, "cannot change zone" },
        { 62, "invalid league" },
        { 63, "invalid idmatch" },
        { 64, "ranking not found" },
        { 65, "idmatch unknown" },
        { 66, "invalid difficulty" },
        { 67, "invalid record type" },
        { 68, "record type unknown" },
        { 69, "challenge of the week unknown" },
        { 70, "replay unknown" },
        { 71, "invalid statistics" },
        { 72, "invalid statistics" },
        { 73, "invalid statistics" },
        { 74, "invalid statistics" },
        { 75, "invalid statistics" },
        { 76, "invalid statistics" },
        { 77, "invalid statistics" },
        { 78, "invalid request" },
        { 79, "You have not enough coppers!" },
        { 80, "wrong login" },
        { 81, "invalid mode" },
        { 82, "code unknown" },
        { 83, "invalid profile" },
        { 84, "self favouriting" },
        { 85, "the server is already your favorite" },
        { 86, "favorite not found" },
        { 87, "invalid medals" },
        { 88, "You have earned too many coppers with medals" },
        { 89, "bad validation infos" },
        { 90, "invalid solo account validation" },
        { 91, "bad solo account validation" },
        { 92, "Account not validated: the characters entered do not match" },
        { 93, "You cannot subscribe to more groups" },
        { 94, "invalid collection" },
        { 95, "A group with the same name already exists" },
        { 96, "same login" },
        { 97, "transaction unknown" },
        { 98, "You are not the correct payer for this transaction" },
        { 99, "The payee is not the good one for this transaction" },
        { 100, "The cost transmitted is not the same as recorded on the master server for this transaction" },
        { 101, "The name transmitted is not the same as recorded on the master server for this transaction" },
        { 102, "The transaction has already been paid" },
        { 103, "Bad packmask" },
        { 104, "Bad game mode" },
        { 105, "Corrupted Datas" },
        { 106, "Please try again later" },
        { 107, "Too many players" },
        { 108, "not connected" },
        { 109, "invalid object type" },
        { 110, "invalid page" },
        { 111, "invalid page count" },
        { 112, "invalid crash address" },
        { 113, "invalid exe" },
        { 114, "cannot open file" },
        { 115, "cannot write file" },
        { 116, "invalid file" },
        { 117, "cannot download" },
        { 118, "no file match" },
        { 119, "invalid session id" },
        { 120, "not connected" },
        { 121, "Wrong password. Please make sure you spelled your password right" },
        { 122, "already connected" },
        { 123, "You have been banned" },
        { 124, "Too many connections: you cannot connect to different accounts simultaneously from the same machine" },
        { 125, "this account does not have rights to perform this action" },
        { 126, "Connection denied, your account is not allowed to connect" },
        { 127, "Server unknown or disconnected" },
        { 128, "Unknown campaign" },
        { 129, "No scores available" },
        { 130, "You cannot create groups" },
        { 131, "You cannot subscribe to groups" },
        { 132, "You cannot do official records" },
        { 133, "You cannot change your zone" },
        { 134, "You cannot use messenger" },
        { 135, "You cannot find players" },
        { 136, "You cannot find servers" },
        { 137, "You cannot manage buddies" },
        { 138, "You cannot manage favorites" },
        { 139, "Invalid moderator login" },
        { 140, "Invitation failed, the server could not send the e-mail to your friend" },
        { 141, "A player with this e-mail is already invited" },
        { 142, "You already ask this player to be your buddy" },
        { 143, "You need to wait a while before adding a removed buddy" },
        { 144, "You have reached the maximum number of buddies allowed. You need to remove a buddy before adding a new one" },
        { 145, "You have reached the maximum number of favourites servers allowed. You need to remove a server before adding a new one" },
        { 146, "You already asked a conversion of your paying account" },
        { 147, "You cannot convert your account again" },
        { 148, "You cannot do account conversions" },
        { 149, "invalid int" },
        { 150, "invalid medium int" },
        { 151, "This key is already associated to a TrackMania United account" },
        { 152, "player online" },
        { 153, "invalid coppers" },
        { 154, "bad identification" },
        { 155, "deletion denied" },
        { 156, "unknown key or account" },
        { 157, "empty parameter" },
        { 158, "unknown key" },
        { 159, "You have no more tries left. Please retry later" },
        { 160, "Invalid distro" }
    };
}
