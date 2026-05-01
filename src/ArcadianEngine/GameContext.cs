using ArcadianEngine.Core;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine;

public class GameContext<G>(Game<G> game) where G : class, IArcadianGame<G>
{
    public Game<G> Game { get; private set; } = game;

    public void InsertGameState<T>() where T : GameState<G>, new()
    {
        T state = new();
        Game.gameStateMachine.AddState(typeof(T).Name, state, this);
    }

    public void InsertSchedule<T>() where T : struct, ISchedule
    {
        Game.schedules.InsertSchedule<T>(Game.world);
    }

    public void RemoveSchedule<T>() where T : struct, ISchedule
    {
        Game.schedules.RemoveSchedule<T>();
    }

    public SystemType InsertSystem<Schedule, SystemType>(SystemType system) where Schedule : struct, ISchedule where SystemType : BaseSystem
    {
        return Game.schedules.InsertSystem<Schedule, SystemType>(system);
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        Game.schedules.RemoveSystem<T>(system);
    }

    public void InsertResource<TRes>(TRes resource) where TRes : class
        => Game.resource_container.InsertResource(resource);

    public TRes GetResource<TRes>() where TRes : class
        => Game.resource_container.GetResource<TRes>();
}
