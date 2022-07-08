using System.Reflection;

namespace TheOmenDen.EventStore.Extensions;
public static class TypeExtensions
{
    public static String GetClassName(this Type type)
    {
        return $"{type.FullName}, {Assembly.GetAssembly(type)?.GetName().Name}";
    }
}