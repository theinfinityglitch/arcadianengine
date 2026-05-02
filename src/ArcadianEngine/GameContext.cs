using Friflo.Engine.ECS.Systems;
using Raylib_cs;

using ArcadianEngine.Core;
using ArcadianEngine.Resources;

namespace ArcadianEngine;

public class GameContext<G>(Game<G> game) where G : class, IArcadianGame<G>
{
    public Game<G> Game { get; private set; } = game;

    public void Quit()
    {
        Game.shouldClose = true;
    }

    public void TogleBorderlessWindow()
    {
        Raylib.ToggleBorderlessWindowed();

        if (Raylib.IsWindowState(ConfigFlags.UndecoratedWindow))
            Raylib.ClearWindowState(ConfigFlags.TopmostWindow);
    }

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

    public TRes GetResource<TRes>(Action<TRes> actions) where TRes : class
    {
        TRes resource = GetResource<TRes>();

        actions(resource);

        return resource;
    }
}
