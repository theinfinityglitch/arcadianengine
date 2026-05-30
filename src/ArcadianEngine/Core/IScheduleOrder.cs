using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Core;

public class ScheduleOrder
{
    public virtual void InsertSchedule<T>() where T : struct, ISchedule { }

    public virtual void RemoveSchedule<T>() where T : struct, ISchedule { }

    public virtual TSystemType InsertSystem<TSchedule, TSystemType>(TSystemType system) where TSchedule : struct, ISchedule where TSystemType : BaseSystem
    {
        throw new InvalidOperationException($"InsertSystem function not implemented for {GetType().Name}");
    }

    public virtual void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule { }

    public virtual void Run() { }
}
