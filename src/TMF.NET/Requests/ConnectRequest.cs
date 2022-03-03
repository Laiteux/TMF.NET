using System.Xml.Serialization;
using TMF.NET.Helpers;

namespace TMF.NET.Requests;

public class ConnectRequest : RequestBase<ConnectRequest>
{
    private const string _rsaPublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCGR7X7yjuTHpicxIUMKvaWzu1SmjpleZdTnB1fLVe6RKAkvuw7OdP9Zr2cqvwHhTyKVy43bJV9uRI9ndGUoEJdM/Lad0nDiCflqhB8a6BgWknxCuRpdzGfFmC8oomVxphrnt1oe9uKOkslrTcjTDU7ZmXQaNccF8InKQxPHUuCFwIDAQAB";
    private const string _rsaPrivateKeyPem = "-----BEGIN RSA PRIVATE KEY-----\nMIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAIZHtfvKO5MemJzE\nhQwq9pbO7VKaOmV5l1OcHV8tV7pEoCS+7Ds50/1mvZyq/AeFPIpXLjdslX25Ej2d\n0ZSgQl0z8tp3ScOIJ+WqEHxroGBaSfEK5Gl3MZ8WYLyiiZXGmGue3Wh724o6SyWt\nNyNMNTtmZdBo1xwXwicpDE8dS4IXAgMBAAECgYBqRGwwBN7a0jbisd+9Pm8B8Gb+\nnRGj5vMsdvsDrKWlwtOd4P6g7GXpP5rFVse3x+iebtojgKpZ4dIeszv+TEnXxNnR\nKwz8IXCJLuezrHParODeBmZdA7N6DGT+TneamwfLG2Q/TKSHqEky6KwsEUXbjBhs\nXwh0hJXDuzGnO+JHIQJBAMT+kEcsDB4wi2phzDEXuq5mU14X1Xk0UjZdjG8zn+Mv\nVDMpW8/26iryMhnEenC6HT6iB53HwSwp0XhI6aISLrECQQCugD80zKsRMiQPdFBN\nxC7pCEKs80I1hAvigoLVvy20hwboxGYQ4JdUSu69S0H2KOgO5d1Ho0ayn6SqOet7\nsz9HAkEAtS48nHkSnCGh2DIij3R3qjdKrdvV5ygMBRx9MTmV8GlzU6rSWq+KJ/2h\nvrlKs6s5goWb4635KRk9IxhMaVe3wQJAB6eyniNYYdG8ST1GBJNVp31oR2QIhIZ9\navtkt/HCFIhT4kQzYgwoN1duL7msdBsJSxYEZg8gm9drBtixnuSXIwJAeW57FJt+\nqZljYIXhjlsXqAmM+21qWehhUWnzKkWfTjT0vMxJtA5p07eKnFsKGDZCKpIcIlma\ntlfvsVk/YCO55g==\n-----END RSA PRIVATE KEY-----";

    private ConnectRequest()
    {
    }

    internal ConnectRequest(GameSession session, string playerKeyLast3characters = null) : base("Connect", new()
    {
        HardwareKey = session.Blowfish.EncryptCBC(Guid.NewGuid().ToString("N").ToUpper().Remove(20).Insert(12, "-")).ToUpper(),
        ValidationKey = playerKeyLast3characters == null ? null : session.Blowfish.EncryptCBC(playerKeyLast3characters.ToUpper()).ToUpper(),
        DedicatedServerDate = "0",
        PublicKey = _rsaPublicKey,
        Login = CryptoHelper.RsaPrivateEncryptToBase64(_rsaPrivateKeyPem, session.Login)
    }, true)
    {
    }

    [XmlElement("hk")]
    public string HardwareKey { get; set; }

    [XmlElement("vk")]
    public string ValidationKey { get; set; }

    [XmlElement("sd")]
    public string DedicatedServerDate { get; set; }

    [XmlElement("pk")]
    public string PublicKey { get; set; }

    [XmlElement("cl")]
    public string Login { get; set; }
}
