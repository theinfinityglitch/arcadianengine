using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using Friflo.Engine.ECS;

using ArcadianEngine.Core;
using ArcadianEngine.Math;
using ArcadianEngine.Resources;
using ArcadianEngine.StateMachines;

namespace ArcadianEngine;

/// <summary>
/// A Arcadian Engine project, may it be a app or a game.
/// </summary>
public class Game<G> where G : class, IArcadianGame<G>
{
    private readonly G game;
    private readonly GameContext<G> context;

    public readonly ResourceContainer resource_container = new();
    public readonly EntityStore world = new();
    public readonly LinearStateMachine<G> gameStateMachine;

    // TODO: Move to the data manager
    private readonly string title;
    private readonly string formated_title;
    private readonly Vector2i windowSize;

    public Game(G game, string title, Vector2i windowSize)
    {
        this.game = game;
        this.title = title;
        this.windowSize = windowSize;
        context = new(this);
        gameStateMachine = new("GameStateMachine", context);

#if DEBUG
        formated_title = title + " [DEBUG]";
#else
        formated_title = title;
#endif

        context.InsertResource(new MainScheduleOrder<G>(context));
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
        rlImGui.Setup();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            rlImGui.Begin();

#if DEBUG
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Save")) Console.WriteLine("Save pressed");
                    if (ImGui.MenuItem("Load")) Console.WriteLine("Load pressed");

                    ImGui.EndMenu();
                }

                string fps_label = $"{Raylib.GetFPS()} FPS";
                float text_width = ImGui.CalcTextSize(fps_label).X;
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - text_width - ImGui.GetStyle().ItemSpacing.X);
                ImGui.Text(fps_label);

                ImGui.EndMainMenuBar();
            }
#endif

            ImGui.ShowDemoWindow();

            game.OnUpdate(context);
            context.GetResource<MainScheduleOrder<G>>().Run();
            gameStateMachine.Update(Raylib.GetFrameTime());
            gameStateMachine.Draw();

            rlImGui.End();
            Raylib.EndDrawing();
        }

        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    protected virtual void Initialize()
    {
        game.OnInitialize(context);
        game.OnLoadContent(context);
        gameStateMachine.Initialize();
    }
}
