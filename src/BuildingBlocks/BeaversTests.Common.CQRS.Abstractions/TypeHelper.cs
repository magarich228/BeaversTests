namespace BeaversTests.Common.CQRS.Abstractions;

public static class TypeHelper
{
    public static string GetTypeName(this Type type)
    {
        return $"{type.Namespace}.{type.Name}"
            .Replace('+', '.')
            .ToLowerInvariant();
    }
}