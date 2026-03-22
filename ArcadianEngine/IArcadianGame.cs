namespace ArcadianEngine;

/// <summary>
/// This is the main loop of a Arcadian Engine game. This has the callbacks to relevant events in the game.
/// </summary>
public interface IArcadianGame
{
    /// <summary>
    /// Called once, when the executable for the game starts and initializes.
    /// </summary>
    public void Initialize()
    {
        this.LoadContent();
    }

    public void LoadContent() { }

    /// <summary>
    /// Called after each update.
    /// </summary>
    public void OnUpdate() { }

    /// <summary>
    /// Called before the draw step.
    /// </summary>
    public void OnDraw() { }

    /// <summary>
    /// Called after each draw.
    /// </summary>
    public void AfterDraw() { }

    /// <summary>
    /// Called before a scene transition.
    /// </summary>
    public void OnSceneTransition() { }

    /// <summary>
    /// Called once the game exits.
    /// </summary>
    public void OnClose() { }
}
