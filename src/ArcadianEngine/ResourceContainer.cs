using System;

namespace ArcadianEngine;

public class ResourceContainer
{
    private readonly Dictionary<Type, object> resources = [];

    public void InsertResource<T>(T resource) where T : class
    {
        resources[typeof(T)] = resource;
    }

    public T GetResource<T>() where T : class
    {
        if (resources.TryGetValue(typeof(T), out var resource))
            return (T)resource;

        throw new InvalidOperationException($"Resource of type {typeof(T).Name} not found.");
    }

    public bool HasResource<T>() where T : class => resources.ContainsKey(typeof(T));

    public bool TryGetResource<T>(out T? resource) where T : class
    {
        if (resources.TryGetValue(typeof(T), out var obj))
        {
            resource = (T)obj;
            return true;
        }
        resource = null;
        return false;
    }
}
