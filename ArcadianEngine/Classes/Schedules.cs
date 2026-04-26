using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Classes;

public class ScheduleOrder
{
    private readonly Dictionary<string, SystemRoot> inner = [];

    public void InsertSchedule<T>(Game cx) where T : struct, ISchedule
    {
        var label = new T();
        SystemRoot schedule = new(cx.world);

        if (!inner.ContainsKey(label.GetLabel()))
            inner.Add(label.GetLabel(), schedule);
        else
            inner[label.GetLabel()] = schedule;
    }

    public void InsertSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        var label = new T();

        if (inner.ContainsKey(label.GetLabel()))
            inner[label.GetLabel()].Add(system);
    }

    public void Run()
    {
        foreach (var schedule in inner)
        {
            schedule.Value.Update(default);
        }
    }
}
