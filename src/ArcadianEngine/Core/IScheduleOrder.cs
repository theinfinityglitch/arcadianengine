using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Core;

public class IScheduleOrder
{
    public virtual void InsertSchedule<T>() where T : struct, ISchedule { }

    public virtual void RemoveSchedule<T>() where T : struct, ISchedule { }

    public virtual SystemType InsertSystem<Schedule, SystemType>(SystemType system) where Schedule : struct, ISchedule where SystemType : BaseSystem
    {
        throw new InvalidOperationException($"InsertSystem function not implemented for {GetType().Name}");
    }

    public virtual void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule { }

    public virtual void Run() { }
}
