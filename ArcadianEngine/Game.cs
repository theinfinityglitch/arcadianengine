using ArcadianEngine.Classes;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Raylib_cs;

namespace ArcadianEngine;

/// <summary>
/// A Arcadian Engine project, may it be a app or a game.
/// </summary>
public class Game<G> where G : class, IArcadianGame<G>
{
    private readonly G game;
    private readonly GameContext<G> context;
    private readonly EntityStore world = new();
    private readonly GameStateMachine<G> gameStateMachine = new();
    private readonly ScheduleOrder schedules = new();

    // TODO: Move to the data manager
    private readonly string title;
    private readonly string formated_title;
    private readonly Vector2i windowSize;

    public Game(G game, string title, Vector2i windowSize) 
    {
        this.game = game;
        this.title = title;
        this.windowSize = windowSize;
        this.context = new(this);

#if DEBUG
        formated_title = title + " [DEBUG]";
#else
        formated_title = title;
#endif

        InsertSchedule<Update>();
        InsertSchedule<Draw>();
    }

    /// <summary>
    /// Initialize the main window and start the game loop.
    /// </summary>
    public void Run()
    {
        Raylib.InitWindow(windowSize.x, windowSize.y, formated_title);

        Raylib.SetTargetFPS(60);

        this.Initialize();
        game.OnUpdate(context);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Raylib.SetWindowTitle($"{formated_title} - {Raylib.GetFPS()} FPS");

            game.OnUpdate(context);
            schedules.Run();
            gameStateMachine.Update(Raylib.GetFrameTime(), context);
            gameStateMachine.Draw(context);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    protected virtual void Initialize()
    {
        game.OnInitialize(context);
        game.OnLoadContent(context);
        gameStateMachine.Initialize(context);
    }

    public void InsertGameState<T>() where T : GameState<G>, new()
    {
        T state = new();
        gameStateMachine.AddState(typeof(T).Name, state, context);
    }

    public void InsertSchedule<T>() where T : struct, ISchedule
    {
        schedules.InsertSchedule<T>(world);
    }

    public void RemoveSchedule<T>() where T : struct, ISchedule
    {
        schedules.RemoveSchedule<T>();
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
