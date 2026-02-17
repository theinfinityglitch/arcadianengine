using ArcadianEngine.Classes;
using ArcadianEngine.Data;
using Raylib_cs;

namespace ArcadianEngine;

/// <summary>
/// A Arcadian Engine project, may it be a app or a game.
/// </summary>
public class Game(IArcadianGame? game, string title, Vector2i windowSize, GameDataManager dataManager)
{
    private readonly IArcadianGame? _game = game;
    private readonly GameDataManager? _gameData = dataManager;

    public Game(IArcadianGame? game, string title, Vector2i windowSize) : this(game, title, windowSize, new GameDataManager(game)) { }

    /// <summary>
    /// Initialize the main window and start the game loop.
    /// </summary>
    public void Run()
    {
        this.Initialize();
        this.LoadContent();

#if DEBUG
        Raylib.InitWindow(windowSize.x, windowSize.y, $"{title} - [DEBUG]");
#else
        Raylib.InitWindow(windowSize.x, windowSize.y, title);
#endif
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    protected virtual void Initialize()
    {
        
    }

    protected virtual void LoadContent()
    {
        // _gameData?.Initialize();
    }
}
