using ArcadianEngine.Classes;
using ArcadianEngine.Data;
using Raylib_cs;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System.Collections.Specialized;

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
    public readonly GameStateMachine? gameStateMachine;
    public readonly Schedules schedules = new();
    readonly string title = title;
    string? formated_title;

    public Game(IArcadianGame? game, string title, Vector2i windowSize) : this(game, title, windowSize, new GameDataManager(game))
    {
        Instance = this;
        world = new EntityStore();
        gameStateMachine = new GameStateMachine("GameStateMachine");

        // schedules.Add("Update", new SystemRoot(world));
        // schedules.Add("Draw", new SystemRoot(world));
    }

    /// <summary>
    /// Initialize the main window and start the game loop.
    /// </summary>
    public void Run()
    {
#if DEBUG
        formated_title = title + " [DEBUG]";
#else
        formated_title = title;
#endif
        Raylib.InitWindow(windowSize.x, windowSize.y, formated_title);

        Raylib.SetTargetFPS(60);

        this.Initialize();
        _game?.OnUpdate();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Raylib.SetWindowTitle($"{formated_title} - {Raylib.GetFPS()} FPS");

            _game?.OnUpdate();
            gameStateMachine?.Update(Raylib.GetFrameTime(), this);
            gameStateMachine?.Draw(this);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    protected virtual void Initialize()
    {
        _game?.Initialize();
        gameStateMachine?.Initialize();
        _game?.LoadContent();
    }
}
