namespace TMF.NET.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        foreach (TSource item in source)
        {
            action(item);
        }
    }
}
