using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Classes;

public class ScheduleOrder
{
    private readonly Dictionary<string, SystemRoot> inner = [];

    public void InsertSchedule<T>(EntityStore world) where T : struct, ISchedule
    {
        SystemRoot schedule = new(world);

        if (!inner.ContainsKey(typeof(T).Name))
            inner.Add(typeof(T).Name, schedule);
        else
            inner[typeof(T).Name] = schedule;
    }

    public void RemoveSchedule<T>() where T : struct, ISchedule
    {
        if (inner.ContainsKey(typeof(T).Name))
            inner.Remove(typeof(T).Name);
    }

    public void InsertSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        if (inner.ContainsKey(typeof(T).Name))
            inner[typeof(T).Name].Add(system);
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        if (inner.ContainsKey(typeof(T).Name))
            inner[typeof(T).Name].Remove(system);
    }

    public void Run()
    {
        foreach (var schedule in inner)
        {
            schedule.Value.Update(default);
        }
    }
}
