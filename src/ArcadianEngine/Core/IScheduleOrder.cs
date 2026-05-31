using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Core;

public interface IScheduleOrder
{
    public void InsertSchedule<T>() where T : struct, ISchedule
    {
    }

    public void RemoveSchedule<T>() where T : struct, ISchedule
    {
    }

    public TSystemType InsertSystem<TSchedule, TSystemType>(TSystemType system)
        where TSchedule : struct, ISchedule where TSystemType : BaseSystem
    {
        throw new InvalidOperationException($"InsertSystem function not implemented for {GetType().Name}");
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
    }

    public void Run()
    {
    }
}