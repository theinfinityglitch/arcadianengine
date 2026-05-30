namespace ArcadianEngine.Core;

public class ResourceContainer
{
    private readonly Dictionary<Type, object> _resources = [];

    public void InsertResource<T>(T resource) where T : class
    {
        _resources[typeof(T)] = resource;
    }

    public T GetResource<T>() where T : class
    {
        if (_resources.TryGetValue(typeof(T), out var resource))
            return (T)resource;

        throw new InvalidOperationException($"Resource of type {typeof(T).Name} not found.");
    }

    public IReadOnlyDictionary<Type, object> GetAllResources() => _resources;

    public bool HasResource<T>() where T : class => _resources.ContainsKey(typeof(T));

    public bool TryGetResource<T>(out T? resource) where T : class
    {
        if (_resources.TryGetValue(typeof(T), out var obj))
        {
            resource = (T)obj;
            return true;
        }
        resource = null;
        return false;
    }
}
