using ArcadianEngine.Classes;
using ArcadianEngine.Data;
using Raylib_cs;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ArcadianEngine.Schedules;

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
    private readonly ScheduleOrder schedules = new();
    readonly string title = title;
    string? formated_title;

    public Game(IArcadianGame? game, string title, Vector2i windowSize) : this(game, title, windowSize, new GameDataManager(game))
    {
        Instance = this;
        world = new EntityStore();
        gameStateMachine = new GameStateMachine("GameStateMachine");

        InsertSchedule<Update>();
        InsertSchedule<Draw>();
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
        _game?.OnUpdate(this);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Raylib.SetWindowTitle($"{formated_title} - {Raylib.GetFPS()} FPS");

            _game?.OnUpdate(this);
            schedules.Run();
            gameStateMachine?.Update(Raylib.GetFrameTime(), this);
            gameStateMachine?.Draw(this);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    protected virtual void Initialize()
    {
        _game?.OnInitialize(this);
        _game?.OnLoadContent(this);
        gameStateMachine?.Initialize(this);
    }

    // TODO: Move functions bellow to a proper "Context"

    public void InsertGameState<T>() where T : GameState, new()
    {
        T state = new();
        gameStateMachine?.AddState(this, typeof(T).Name, state);
    }

    public void InsertSchedule<T>() where T : struct, ISchedule
    {
        schedules.InsertSchedule<T>(this);
    }

    public void RemoveSchedule<T>() where T : struct, ISchedule
    {
        schedules.RemoveSchedule<T>(this);
    }

    public void InsertSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        schedules.InsertSystem<T>(system);
    }

    public void RemoveSystem<T>(BaseSystem system) where T : struct, ISchedule
    {
        schedules.RemoveSystem<T>(system);
    }
}
