namespace ArcadianEngine.Utils;

public static class TypeExtensions
{
    public static string GetFriendlyName(this Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        // Remove the `n suffix
        var baseName = type.Name[..type.Name.IndexOf('`')];

        // Get generic arguments and format them
        var genericArgs = type.GetGenericArguments()
                              .Select(t => t.GetFriendlyName());

        return $"{baseName}<{string.Join(", ", genericArgs)}>";
    }
}