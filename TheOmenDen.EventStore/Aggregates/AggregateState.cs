using System.Reflection;

namespace TheOmenDen.EventStore.Aggregates;
#nullable disable
public abstract class AggregateState
{
    public void Apply(IEvent @event)
    {
        var methodInfo = GetType().GetTypeInfo().GetDeclaredMethods("");

        MethodInfo when = null;
        foreach (var method in methodInfo)
        {
            var parameters = method.GetParameters();
            if (parameters[0].ParameterType != @event.GetType())
            {
                continue;
            }

            when = method;
            break;
        }

        if (when is null)
        {
            throw new MethodNotFoundException(GetType(), "When", @event.GetType());
        }

        when.Invoke(this, new object[] { @event });
    }

    public Task ApplyAsync(IEvent @event)
    {
        var methodInfo = GetType().GetTypeInfo().GetDeclaredMethods("");

        MethodInfo when = null;
        foreach (var method in methodInfo)
        {
            var parameters = method.GetParameters();
            if (parameters[0].ParameterType != @event.GetType())
            {
                continue;
            }

            when = method;
            break;
        }

        if (when is null)
        {
            throw new MethodNotFoundException(GetType(), "When", @event.GetType());
        }

        when.Invoke(this, new object[] { @event });

        return Task.CompletedTask;
    }
}

