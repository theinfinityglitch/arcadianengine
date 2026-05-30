using ArcadianEngine.Core;
using ArcadianEngine.Resources;
using Friflo.Engine.ECS.Systems;
using Raylib_cs;

namespace ArcadianEngine;

public class GameContext<TG>(Game<TG> game) where TG : class, IArcadianGame<TG>
{
    public Game<TG> game { get; private set; } = game;

    public void Quit()
    {
        game.ShouldClose = true;
    }

    public bool IsBorderlessWindow()
    {
        return Raylib.IsWindowState(ConfigFlags.BorderlessWindowMode);
    }

    public void ToggleBorderlessWindow()
    {
        Raylib.ToggleBorderlessWindowed();

        if (IsBorderlessWindow())
            Raylib.ClearWindowState(ConfigFlags.TopmostWindow);
    }

    public void InsertGameState<T>(T state) where T : State<TG>, new()
    {
        game.GameStateMachine.AddState(state);
    }

    public TSystemType InsertSystem<TSchedule, TSystemType>(TSystemType system) where TSchedule : struct, ISchedule where TSystemType : BaseSystem
    {
        return GetResource<MainScheduleOrder<TG>>().InsertSystem<TSchedule, TSystemType>(system);
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        GetResource<MainScheduleOrder<TG>>().RemoveSystem<T>(system);
    }

    public void InsertResource<TRes>(TRes resource) where TRes : class
        => game.ResourceContainer.InsertResource(resource);

    public TRes GetResource<TRes>() where TRes : class
        => game.ResourceContainer.GetResource<TRes>();

    public bool TryGetResource<TRes>(out TRes? resource) where TRes : class
        => game.ResourceContainer.TryGetResource(out resource);

    public IReadOnlyDictionary<Type, object> GetAllResources()
        => game.ResourceContainer.GetAllResources();

    public TRes GetResource<TRes>(Action<TRes> actions) where TRes : class
    {
        var resource = GetResource<TRes>();

        actions(resource);

        return resource;
    }
}
