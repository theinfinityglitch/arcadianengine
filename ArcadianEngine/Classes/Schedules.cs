using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Classes;

public class ScheduleOrder
{
    private readonly Dictionary<string, SystemRoot> inner = [];

    public void InsertSchedule<T>(Game cx) where T : struct, ISchedule
    {
        SystemRoot schedule = new(cx.world);

        if (!inner.ContainsKey(typeof(T).Name))
            inner.Add(typeof(T).Name, schedule);
        else
            inner[typeof(T).Name] = schedule;
    }

    public void InsertSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        if (inner.ContainsKey(typeof(T).Name))
            inner[typeof(T).Name].Add(system);
    }

    public void Run()
    {
        foreach (var schedule in inner)
        {
            schedule.Value.Update(default);
        }
    }
}
