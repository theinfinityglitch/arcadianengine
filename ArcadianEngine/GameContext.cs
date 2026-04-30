using ArcadianEngine.Classes;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine;

public class GameContext<G>(Game<G> app) where G : class, IArcadianGame<G>
{
    public Game<G> App { get; private set; } = app;

    public void InsertGameState<T>() where T : GameState<G>, new()
    {
        App.InsertGameState<T>();
    }

    public void InsertSchedule<T>() where T : struct, ISchedule
    {
        App.InsertSchedule<T>();
    }

    public void RemoveSchedule<T>() where T : struct, ISchedule
    {
        App.RemoveSchedule<T>();
    }

    public void InsertSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        App.InsertSystem<T>(system);
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        App.RemoveSystem<T>(system);
    }
}
