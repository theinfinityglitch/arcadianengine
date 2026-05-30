namespace ArcadianEngine;

/// <summary>
/// This is the main loop of a Arcadian Engine game. This has the callbacks to relevant events in the game.
/// </summary>
public interface IArcadianGame<TSelf> where TSelf : class, IArcadianGame<TSelf>
{
    /// <summary>
    /// Called once, when the executable for the game starts and initializes.
    /// </summary>
    public void OnInitialize(GameContext<TSelf> cx) { }

    public void OnLoadContent(GameContext<TSelf> cx) { }

    /// <summary>
    /// Called after each update.
    /// </summary>
    public void OnUpdate(GameContext<TSelf> cx) { }

    /// <summary>
    /// Called before the draw step.
    /// </summary>
    public void OnDraw(GameContext<TSelf> cx) { }

    /// <summary>
    /// Called after each draw.
    /// </summary>
    public void OnAfterDraw(GameContext<TSelf> cx) { }

    /// <summary>
    /// Called before a scene transition.
    /// </summary>
    public void OnSceneTransition(GameContext<TSelf> cx) { }

    /// <summary>
    /// Called once the game exits.
    /// </summary>
    public void OnClose(GameContext<TSelf> cx) { }
}
