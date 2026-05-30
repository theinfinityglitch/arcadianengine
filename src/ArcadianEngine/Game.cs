using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

using Friflo.Engine.ECS;
using IconFonts;
using ImGuiNET;
using Raylib_cs;

using ArcadianEngine.Core;
using ArcadianEngine.Math;
using ArcadianEngine.Resources;
using ArcadianEngine.StateMachines;
using ArcadianEngine.Systems;

namespace ArcadianEngine;

/// An Arcadian Engine project, may it be an app or a game.
public sealed class Game<TG> where TG : class, IArcadianGame<TG>
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
        _formatedTitle = title + " [DEBUG]";
#else
        formated_title = title;
#endif

        _context.InsertResource(new MainScheduleOrder<TG>(_context));
        _context.InsertResource(new RenderPipeline(windowSize));
        _context.InsertResource(new WorldHierarchyDebug<TG>(_context));
        _context.InsertResource(new ImGuiConsole());

        _context.GetResource<MainScheduleOrder<TG>>().InsertSystem<PostUpdate, TransformPropagationSystem>(new TransformPropagationSystem());
    }
    
    /// Initialize the main window and start the game loop.
    public void Run()
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
            LoadEmbeddedFont("default_font.ttf", 16.0f * dpiScale);
            LoadEmbeddedIconFont("lucide.ttf", 12.0f * dpiScale, Lucide.IconMin, Lucide.IconMax);
        });

        Initialize();
        Update(); // Update the game once at start

        while (!ShouldClose)
        {
            Update();
        }

        ImGuiRaylibBackend.Shutdown();
        Raylib.CloseWindow();
    }

    private static void LoadEmbeddedFont(string resourceName, float fontSize)
    {
        // 1. Get the current assembly
        var assembly = Assembly.GetExecutingAssembly();

        // 2. Open the resource stream
        // resourceName is usually "YourNamespace.YourFolder.FontFile.ttf"
        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new Exception($"Resource {resourceName} not found.");

        // 3. Read stream into a byte array
        var buffer = new byte[stream.Length];
        stream.ReadExactly(buffer, 0, buffer.Length);

        // 4. Pass the data to ImGui
        // We use 'fixed' to get a pointer to the managed byte array
        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            var ptr = handle.AddrOfPinnedObject();
            ImGui.GetIO().Fonts.AddFontFromMemoryTTF(ptr, buffer.Length, fontSize);
            ImGuiRaylibBackend.ReloadFonts();
        }
        finally
        {
            handle.Free();
        }
    }

    private static unsafe void LoadEmbeddedIconFont(string resourceName, float fontSize, ushort iconMin, ushort iconMax)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new Exception($"Embedded resource '{resourceName}' not found.");

        var buffer = new byte[stream.Length];
        stream.ReadExactly(buffer, 0, buffer.Length);

        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            var ptr = handle.AddrOfPinnedObject();
            var io = ImGui.GetIO();

            // Glyph range for FA5
            ushort[] ranges = [iconMin, iconMax, 0];
            var rangeHandle = GCHandle.Alloc(ranges, GCHandleType.Pinned);
            try
            {
                ImFontConfigPtr cfg = ImGuiNative.ImFontConfig_ImFontConfig();
                cfg.MergeMode = true;  // merge into previous font
                cfg.PixelSnapH = true; // cleaner rendering for icons
                cfg.GlyphMinAdvanceX = fontSize; // monospace icons

                io.Fonts.AddFontFromMemoryTTF(
                    ptr,
                    buffer.Length,
                    fontSize,
                    cfg,
                    rangeHandle.AddrOfPinnedObject()
                );
            }
            finally
            {
                rangeHandle.Free();
            }
        }
        finally
        {
            handle.Free();
        }
    }

    private void Initialize()
    {
        _game.OnInitialize(_context);
        _game.OnLoadContent(_context);
        GameStateMachine.Initialize();
    }

    private void Update()
    {
        ImGuiRaylibBackend.Begin();
        _game.OnUpdate(_context);
        GameStateMachine.Update(Raylib.GetFrameTime());
        _context.GetResource<MainScheduleOrder<TG>>().Run();
        GameStateMachine.Draw();

        _context.TryGetResource<RenderPipeline>(out var rp);

        RenderTexture2D frame = new();

        if (rp != null) frame = rp.Flush();

        Raylib.BeginDrawing();

        rp?.PresentToScreen(frame);

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
                if (ImGui.MenuItem("Toggle borderless", null, _context.IsBorderlessWindow())) _context.ToggleBorderlessWindow();
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

        Raylib.UnloadRenderTexture(frame);

        if (Raylib.WindowShouldClose()) _context.Quit();
    }
}
