using ArcadianEngine.Classes;
using ArcadianEngine.Data;
using Raylib_cs;
using Friflo.Engine.ECS;

namespace ArcadianEngine;

/// <summary>
/// A Arcadian Engine project, may it be a app or a game.
/// </summary>
public partial class Game(IArcadianGame? game, string title, Vector2i windowSize, GameDataManager dataManager)
{
    public static Game? Instance { get; private set; } = null;
    private readonly IArcadianGame? _game = game;
    private readonly GameDataManager? _gameData = dataManager;
    public readonly EntityStore? world;

    public Game(IArcadianGame? game, string title, Vector2i windowSize) : this(game, title, windowSize, new GameDataManager(game))
    {
        Instance = this;
        world = new EntityStore();
    }

    /// <summary>
    /// Initialize the main window and start the game loop.
    /// </summary>
    public void Run()
    {
#if DEBUG
        Raylib.InitWindow(windowSize.x, windowSize.y, $"{title} - [DEBUG]");
#else
        Raylib.InitWindow(windowSize.x, windowSize.y, title);
#endif

        Raylib.SetTargetFPS(60);

        this.Initialize();
        _game?.OnUpdate();

        while (!Raylib.WindowShouldClose())
        {
            _game?.OnUpdate();

            Raylib.BeginDrawing();

            _game?.OnDraw();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    protected virtual void Initialize()
    {
        _game?.Initialize();
        _game?.LoadContent();
    }
}
