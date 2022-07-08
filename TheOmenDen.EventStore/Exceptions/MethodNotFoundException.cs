namespace TheOmenDen.EventStore.Exceptions;
internal class MethodNotFoundException: Exception
{
    public MethodNotFoundException(Type classType, String methodName, Type parameterType)
    : base($"This class ({classType.FullName}) has no method named \"{methodName}\" that takes this parameter ({parameterType}).")
    {
    }
}

