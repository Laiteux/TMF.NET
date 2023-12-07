namespace TMF.NET;

public class TmfGameServer
{
    private readonly string _value;
    private TmfGameServer(string value) { _value = value; }

    public static readonly TmfGameServer United = new("game.trackmaniaforever.com");
    public static readonly TmfGameServer Nations = new("game2.trackmaniaforever.com");
    internal static readonly TmfGameServer Default = Nations;

    public static implicit operator string(TmfGameServer gameServer)
    {
        return gameServer._value;
    }

    public static implicit operator TmfGameServer(string gameServer)
    {
        return gameServer == United ? United : Nations;
    }

    public override string ToString()
        => _value;
}
