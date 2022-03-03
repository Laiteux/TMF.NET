namespace TMF.NET.Extensions;

internal static class StringExtensions
{
    public static Stream ToStream(this string str)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);

        writer.Write(str);
        writer.Flush();

        stream.Position = 0;

        return stream;
    }
}
