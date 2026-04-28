namespace ArcadianEngine;

/// <summary>
/// This is the main loop of a Arcadian Engine game. This has the callbacks to relevant events in the game.
/// </summary>
public interface IArcadianGame
{
    /// <summary>
    /// Called once, when the executable for the game starts and initializes.
    /// </summary>
    public void OnInitialize(Game cx) { }

    public void OnLoadContent(Game cx) { }

    /// <summary>
    /// Called after each update.
    /// </summary>
    public void OnUpdate(Game cx) { }

    /// <summary>
    /// Called before the draw step.
    /// </summary>
    public void OnDraw(Game cx) { }

    /// <summary>
    /// Called after each draw.
    /// </summary>
    public void OnAfterDraw(Game cx) { }

    /// <summary>
    /// Called before a scene transition.
    /// </summary>
    public void OnSceneTransition(Game cx) { }

    /// <summary>
    /// Called once the game exits.
    /// </summary>
    public void OnClose(Game cx) { }
}
