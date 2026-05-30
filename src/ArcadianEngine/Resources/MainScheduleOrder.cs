using ArcadianEngine.Core;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Resources;

public class MainScheduleOrder<TG> : ScheduleOrder where TG : class, IArcadianGame<TG>
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

    public override void InsertSchedule<T>()
    {
        SystemRoot schedule = new(_context.game.World);

        if (!_inner.TryAdd(typeof(T).Name, schedule))
            _inner[typeof(T).Name] = schedule;
    }

    public override void RemoveSchedule<T>()
    {
        if (_inner.ContainsKey(typeof(T).Name))
            _inner.Remove(typeof(T).Name);
    }

    public override TSystemType InsertSystem<TSchedule, TSystemType>(TSystemType system)
    {
        if (_inner.TryGetValue(typeof(TSchedule).Name, out var schedule))
            schedule.Add(system);

        return system;
    }

    public override void RemoveSystem<T>(BaseSystem system)
    {
        if (_inner.TryGetValue(typeof(T).Name, out var schedule))
            schedule.Remove(system);
    }

    public override void Run()
    {
        foreach (var schedule in _inner)
        {
            schedule.Value.Update(default);
        }
    }
}
