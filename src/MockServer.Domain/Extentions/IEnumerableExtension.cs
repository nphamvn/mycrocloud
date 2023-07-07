namespace MockServer.Domain.Extentions;

public static class IEnumerableExtension
{
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> collection)
    {
        return collection ?? Enumerable.Empty<T>();
    }
}
