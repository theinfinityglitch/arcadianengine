using ArcadianEngine.Core;
using ArcadianEngine.Resources;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine;

public class GameContext<G>(Game<G> game) where G : class, IArcadianGame<G>
{
    public Game<G> Game { get; private set; } = game;

    public void InsertGameState<T>(T state) where T : State<G>, new()
    {
        Game.gameStateMachine.AddState(state);
    }

    public SystemType InsertSystem<Schedule, SystemType>(SystemType system) where Schedule : struct, ISchedule where SystemType : BaseSystem
    {
        return GetResource<MainScheduleOrder<G>>().InsertSystem<Schedule, SystemType>(system);
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        GetResource<MainScheduleOrder<G>>().RemoveSystem<T>(system);
    }

    public void InsertResource<TRes>(TRes resource) where TRes : class
        => Game.resource_container.InsertResource(resource);

    public TRes GetResource<TRes>() where TRes : class
        => Game.resource_container.GetResource<TRes>();
}
