using System.Numerics;
using Friflo.Engine.ECS;
using IconFonts;
using ImGuiNET;
using Raylib_cs;
using ArcadianEngine.Core;
using ArcadianEngine.Math;
using ArcadianEngine.Resources;
using ArcadianEngine.StateMachines;

namespace ArcadianEngine;

/// <summary>
/// This class is the entry point for most games. Handles setting up a window and graphics and runs a game loop
/// </summary>
/// <typeparam name="TG">This is the main loop of an arcadian game. This has the callbacks to relevant events in the game.</typeparam>
public partial class Game<TG> where TG : class, IArcadianGame<TG>
{
    private readonly TG _game;
    private readonly GameContext<TG> _context;

    public readonly ResourceContainer ResourceContainer = new();
    public readonly EntityStore World = new();
    public readonly LinearStateMachine<TG> GameStateMachine;
    public bool ShouldClose = false;

    // TODO: Move to the data manager
    private readonly string _title;
    private readonly string _formatedTitle;
    private readonly Vector2I _windowSize;

    private bool _drawWorldInspector;
    private bool _drawConsole;

    public Game(TG game, string title, Vector2I windowSize)
    {
        _game = game;
        _title = title;
        _windowSize = windowSize;
        _context = new GameContext<TG>(this);
        GameStateMachine = new LinearStateMachine<TG>("GameStateMachine", _context);

#if DEBUG
        _formatedTitle = _title + " [DEBUG]";
#else
        formated_title = _title;
#endif
    }

    ~Game()
    {
        ResourceContainer.Dispose();
    }

    /// Initialize the main window and start the game loop.
    public void Run()
    {
        DoInitialize();
        BeginRun();
        DoUpdate(); // Update the game once at start

        while (!ShouldClose) DoUpdate();

        EndRun();

        ImGuiRaylibBackend.Shutdown();
        Raylib.CloseWindow();
    }

    private void DoInitialize()
    {
        Raylib.SetConfigFlags(ConfigFlags.HighDpiWindow);
        Raylib.InitWindow(_windowSize.X, _windowSize.Y, _formatedTitle);
        Raylib.SetTargetFPS(60);

        var dpiScale = Raylib.GetWindowScaleDPI().X;

        // Setup ImGui, load the engine default font (Roboto) and enable docking
        ImGuiRaylibBackend.Setup(() =>
        {
            ImGuiRaylibBackend.LoadDefaultFont = false;
            var style = ImGui.GetStyle();
            style.ScaleAllSizes(dpiScale);
            style.FramePadding = new Vector2(ImGui.GetStyle().FramePadding.X, 4.0f * Raylib.GetWindowScaleDPI().X);
            style.TabRounding = 0.0f;
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            ImGuiRaylibBackend.LoadEmbeddedFont("default_font.ttf", 16.0f * dpiScale);
            ImGuiRaylibBackend.LoadEmbeddedIconFont("lucide.ttf", 12.0f * dpiScale, Lucide.IconMin, Lucide.IconMax);
        });

        Initialize();
        _game.OnInitialize(_context);

        LoadContent();
        _game.OnLoadContent(_context);

        GameStateMachine.Initialize();
    }

    private void DoUpdate()
    {
        ImGuiRaylibBackend.Begin();

        Update(Raylib.GetFrameTime());
        if (BeginDraw()) Draw(Raylib.GetFrameTime());

        _game.OnUpdate(_context);
        GameStateMachine.Update(Raylib.GetFrameTime());
        _context.GetResource<MainScheduleOrder<TG>>().Run();
        GameStateMachine.Draw();

        Raylib.BeginDrawing();

        EndDraw();

        // Set up the ImGui dockspace
        var dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode;
        ImGui.DockSpaceOverViewport(0, ImGui.GetMainViewport(), dockspaceFlags);

#if DEBUG
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Game"))
            {
                if (ImGui.MenuItem("Quit")) _context.Quit();
                ImGui.SetItemTooltip("Stops the game loop and clear the contexts");

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Window"))
            {
                if (ImGui.MenuItem("Toggle borderless", null, _context.IsBorderlessWindow()))
                    _context.ToggleBorderlessWindow();
                ImGui.SetItemTooltip("Resizes window to match monitor resolution or make it floating");

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Debug"))
            {
                ImGui.MenuItem("Inspector", null, ref _drawWorldInspector);
                ImGui.MenuItem("Console", null, ref _drawConsole);

                ImGui.EndMenu();
            }

            var fpsLabel = $"{Raylib.GetFPS()} FPS";
            var textWidth = ImGui.CalcTextSize(fpsLabel).X;
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - textWidth - ImGui.GetStyle().ItemSpacing.X);
            ImGui.Text(fpsLabel);

            ImGui.EndMainMenuBar();
        }

        if (_drawWorldInspector)
            _context.GetResource<WorldHierarchyDebug<TG>>().Draw();
        if (_drawConsole)
            _context.GetResource<ImGuiConsole>().Draw();
#endif

        ImGuiRaylibBackend.End();
        Raylib.EndDrawing();

        if (_context.TryGetResource<RenderPipeline>(out var pipeline)) pipeline.Dispose();

        if (Raylib.WindowShouldClose()) _context.Quit();
    }
}