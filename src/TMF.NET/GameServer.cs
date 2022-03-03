namespace TMF.NET;

public class GameServer
{
    private readonly string _value;
    private GameServer(string value) { _value = value; }

    public static readonly GameServer United = new("game.trackmaniaforever.com");
    public static readonly GameServer Nations = new("game2.trackmaniaforever.com");
    internal static readonly GameServer Default = Nations;

    public static implicit operator string(GameServer gameServer)
    {
        return gameServer._value;
    }

    public static implicit operator GameServer(string gameServer)
    {
        return gameServer == United ? United : Nations;
    }

    public override string ToString() => _value;
}
