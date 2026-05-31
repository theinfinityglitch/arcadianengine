using ArcadianEngine.Resources;
using ArcadianEngine.Systems;

namespace ArcadianEngine;

public partial class Game<TG> where TG : class, IArcadianGame<TG>
{
    /// <summary>
    /// Override this to initialize the game and load any needed non-graphical resources.
    /// </summary>
    protected virtual void Initialize()
    {
        _context.InsertResource(new MainScheduleOrder<TG>(_context));
        _context.InsertResource(new RenderPipeline(_windowSize));
        _context.InsertResource(new WorldHierarchyDebug<TG>(_context));
        _context.InsertResource(new ImGuiConsole());

        _context.GetResource<MainScheduleOrder<TG>>()
            .InsertSystem<PostUpdate, TransformPropagationSystem>(new TransformPropagationSystem());
    }

    /// <summary>
    /// Override this to load graphical resources required by the game.
    /// </summary>
    protected virtual void LoadContent()
    {
    }

    /// <summary>
    /// Called after <see cref="Initialize"/>, but before the first call to <see cref="Update"/>.
    /// </summary>
    protected virtual void BeginRun()
    {
    }

    protected virtual void Update(float deltaTime)
    {
    }

    protected virtual bool BeginDraw()
    {
        return true;
    }

    /// <summary>
    /// Override this to render your game.
    /// </summary>
    protected virtual void Draw(float deltaTime)
    {
    }

    protected virtual void EndDraw()
    {
        if (!_context.TryGetResource<RenderPipeline>(out var rp)) return;

        var frame = rp.Flush();

        rp.PresentToScreen(frame);
    }

    /// <summary>
    /// Called when the game loop has been terminated before exiting.
    /// </summary>
    protected virtual void EndRun()
    {
    }

    protected virtual void UnloadContent()
    {
    }

    /// <summary>
    /// Override this to unload graphical resources loaded by the game.
    /// </summary>
    protected virtual void Load()
    {
    }
}