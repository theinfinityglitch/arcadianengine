using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

using ArcadianEngine.Core;

namespace ArcadianEngine.Resources;

public class MainScheduleOrder<G> : IScheduleOrder where G : class, IArcadianGame<G>
{
    private readonly Dictionary<string, SystemRoot> inner = [];
    private readonly GameContext<G> context;

    public MainScheduleOrder(GameContext<G> cx)
    {
        context = cx;

        InsertSchedule<PreUpdate>();
        InsertSchedule<Update>();
        InsertSchedule<PostUpdate>();
        InsertSchedule<Draw>();
    }

    public override void InsertSchedule<T>()
    {
        SystemRoot schedule = new(context.Game.world);

        if (!inner.TryAdd(typeof(T).Name, schedule))
            inner[typeof(T).Name] = schedule;
    }

    public override void RemoveSchedule<T>()
    {
        if (inner.ContainsKey(typeof(T).Name))
            inner.Remove(typeof(T).Name);
    }

    public override SystemType InsertSystem<Schedule, SystemType>(SystemType system)
    {
        if (inner.TryGetValue(typeof(Schedule).Name, out var schedule))
            schedule.Add(system);

        return system;
    }

    public override void RemoveSystem<T>(BaseSystem system)
    {
        if (inner.TryGetValue(typeof(T).Name, out var schedule))
            schedule.Remove(system);
    }

    public override void Run()
    {
        foreach (var schedule in inner)
        {
            schedule.Value.Update(default);
        }
    }
}
