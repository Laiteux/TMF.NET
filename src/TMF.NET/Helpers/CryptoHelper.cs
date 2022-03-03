using System.Text;
using OpenSSL.Crypto;
using TMF.NET.Extensions;

namespace TMF.NET.Helpers;

internal static class CryptoHelper
{
    private static readonly object _rsaLock = new();

    public static string MD5(string input)
    {
        // Can't cache/reuse because it doesn't support multi-threading very well
        var md5 = System.Security.Cryptography.MD5.Create();

        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        var stringBuilder = new StringBuilder();
        hashBytes.ForEach(b => stringBuilder.Append((string?)b.ToString("x2")));

        return stringBuilder.ToString();
    }

    public static string RsaPrivateEncryptToBase64(string privateKeyPem, string input, RSA.Padding padding = RSA.Padding.PKCS1)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] encryptBytes;

        // Because it doesn't support multi-threading very well
        lock (_rsaLock)
        {
            var rsa = CryptoKey.FromPrivateKey(privateKeyPem, null).GetRSA();

            encryptBytes = rsa.PrivateEncrypt(inputBytes, padding);
        }

        return Convert.ToBase64String(encryptBytes);
    }
}
