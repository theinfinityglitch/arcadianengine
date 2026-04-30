namespace ArcadianEngine;

/// <summary>
/// This is the main loop of a Arcadian Engine game. This has the callbacks to relevant events in the game.
/// </summary>
public interface IArcadianGame<Self> where Self : class, IArcadianGame<Self>
{
    /// <summary>
    /// Called once, when the executable for the game starts and initializes.
    /// </summary>
    public void OnInitialize(GameContext<Self> cx){ }

    public void OnLoadContent(GameContext<Self> cx) { }

    /// <summary>
    /// Called after each update.
    /// </summary>
    public void OnUpdate(GameContext<Self> cx) { }

    /// <summary>
    /// Called before the draw step.
    /// </summary>
    public void OnDraw(GameContext<Self> cx) { }

    /// <summary>
    /// Called after each draw.
    /// </summary>
    public void OnAfterDraw(GameContext<Self> cx) { }

    /// <summary>
    /// Called before a scene transition.
    /// </summary>
    public void OnSceneTransition(GameContext<Self> cx) { }

    /// <summary>
    /// Called once the game exits.
    /// </summary>
    public void OnClose(GameContext<Self> cx) { }
}
