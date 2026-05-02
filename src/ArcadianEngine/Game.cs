using System.Reflection;

using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using Friflo.Engine.ECS;

using ArcadianEngine.Core;
using ArcadianEngine.Math;
using ArcadianEngine.Resources;
using ArcadianEngine.StateMachines;
using System.Runtime.InteropServices;

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
    public bool shouldClose = false;

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

        float dpiScale = Raylib.GetWindowScaleDPI().X;

        // Setup ImGui, load the engine default font (Roboto) and enable docking
        rlImGui.Setup();
        ImGui.GetIO().Fonts.Clear();
        ImGui.GetStyle().ScaleAllSizes(dpiScale);
        LoadEmbeddedFont("default_font.ttf", 16.0f * dpiScale);
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        Initialize();
        Update(); // Update the game once at start

        while (!shouldClose)
        {
            Update();
        }

        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    protected void LoadEmbeddedFont(string resourceName, float fontSize)
    {
        // 1. Get the current assembly
        var assembly = Assembly.GetExecutingAssembly();

        // 2. Open the resource stream
        // resourceName is usually "YourNamespace.YourFolder.FontFile.ttf"
        using Stream? stream = assembly.GetManifestResourceStream(resourceName) ?? throw new Exception($"Resource {resourceName} not found.");

        // 3. Read stream into a byte array
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        // 4. Pass the data to ImGui
        // We use 'fixed' to get a pointer to the managed byte array
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            IntPtr ptr = handle.AddrOfPinnedObject();
            ImGui.GetIO().Fonts.AddFontFromMemoryTTF(ptr, buffer.Length, fontSize);
            rlImGui.ReloadFonts();
        }
        finally
        {
            handle.Free();
        }
    }

    protected virtual void Initialize()
    {
        game.OnInitialize(context);
        game.OnLoadContent(context);
        gameStateMachine.Initialize();
    }

    protected virtual void Update()
    {
        Raylib.BeginDrawing();
        rlImGui.Begin();

        // Setup the ImGui dockspace
        ImGuiDockNodeFlags dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode;
        ImGui.DockSpaceOverViewport(0, ImGui.GetMainViewport(), dockspaceFlags);

#if DEBUG
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Game"))
            {
                if (ImGui.MenuItem("Quit")) context.Quit();
                ImGui.SetItemTooltip("Stops the game loop and clear the contexts");

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Window"))
            {
                if (ImGui.MenuItem("Toggle borderless", null, context.IsBorderlessWindow())) context.TogleBorderlessWindow();
                ImGui.SetItemTooltip("Resizes window to match monitor resolution or make it floating");

                ImGui.EndMenu();
            }

            string fps_label = $"{Raylib.GetFPS()} FPS";
            float text_width = ImGui.CalcTextSize(fps_label).X;
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - text_width - ImGui.GetStyle().ItemSpacing.X);
            ImGui.Text(fps_label);

            ImGui.EndMainMenuBar();
        }
#endif

        game.OnUpdate(context);
        gameStateMachine.Update(Raylib.GetFrameTime());
        context.GetResource<MainScheduleOrder<G>>().Run();
        gameStateMachine.Draw();

        rlImGui.End();
        Raylib.EndDrawing();

        if (Raylib.WindowShouldClose()) context.Quit();
    }
}
