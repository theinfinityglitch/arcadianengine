using ArcadianEngine.Classes;
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

    public void InsertSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        Game.schedules.InsertSystem<T>(system);
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        Game.schedules.RemoveSystem<T>(system);
    }
}
