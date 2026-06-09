using ArcadianEngine.Core;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Resources;

public sealed class MainScheduleOrder<TG> : IScheduleOrder, IDisposable
    where TG : class, IArcadianGame<TG>
{
    private readonly Dictionary<string, SystemRoot> _inner = [];
    private readonly GameContext<TG> _context;

    public MainScheduleOrder(GameContext<TG> cx)
    {
        _context = cx;

        InsertSchedule<PreUpdate>();
        InsertSchedule<Update>();
        InsertSchedule<PostUpdate>();
        InsertSchedule<Draw>();
    }

    public void InsertSchedule<T>()
        where T : struct, ISchedule
    {
        SystemRoot schedule = new(_context.Game.World);

        if (!_inner.TryAdd(typeof(T).Name, schedule))
            _inner[typeof(T).Name] = schedule;
    }

    public void RemoveSchedule<T>()
        where T : struct, ISchedule
    {
        _inner.Remove(typeof(T).Name);
    }

    public TSystemType InsertSystem<TSchedule, TSystemType>(TSystemType system)
        where TSchedule : struct, ISchedule
        where TSystemType : BaseSystem
    {
        if (_inner.TryGetValue(typeof(TSchedule).Name, out var schedule))
            schedule.Add(system);

        return system;
    }

    public void RemoveSystem<T>(BaseSystem system)
        where T : struct, ISchedule
    {
        if (_inner.TryGetValue(typeof(T).Name, out var schedule))
            schedule.Remove(system);
    }

    public void Run()
    {
        foreach (var schedule in _inner)
            schedule.Value.Update(default);
    }

    public void Dispose()
    {
        _inner.Clear();
    }
}

