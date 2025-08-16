namespace DigitalPetApp.Services;

public static class ServiceRegistry
{
    private static readonly Dictionary<Type, object> services = new();

    public static void Register<T>(T instance) where T : class
    {
        services[typeof(T)] = instance;
    }

    public static T Resolve<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out var value)) return (T)value;
        throw new InvalidOperationException($"Service of type {typeof(T).Name} not registered.");
    }
}
