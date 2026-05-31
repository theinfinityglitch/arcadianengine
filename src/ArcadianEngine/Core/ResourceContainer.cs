using System.Diagnostics.CodeAnalysis;

namespace ArcadianEngine.Core;

public sealed class ResourceContainer : IDisposable
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

    public IReadOnlyDictionary<Type, object> GetAllResources()
    {
        return _resources;
    }

    public bool HasResource<T>() where T : class
    {
        return _resources.ContainsKey(typeof(T));
    }

    public bool TryGetResource<T>([MaybeNullWhen(false)] out T resource) where T : class
    {
        if (_resources.TryGetValue(typeof(T), out var obj))
        {
            resource = (T)obj;
            return true;
        }

        resource = null;
        return false;
    }

    public void Dispose()
    {
        foreach (var resource in _resources.Values)
            if (resource is IDisposable disposable)
                disposable.Dispose();

        _resources.Clear();
    }
}