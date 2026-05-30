using System.Numerics;
using ArcadianEngine.Utils;
using IconFonts;
using ImGuiNET;

public class ImGuiConsole
{
    private readonly ImGuiConsoleWriter _writer = new();
    private bool _autoScroll = true;

    public ImGuiConsole()
    {
        // Redirect Console.Out to our writer
        Console.SetOut(_writer);
    }

    public void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(600, 200), ImGuiCond.FirstUseEver);

        if (!ImGui.Begin($"{Lucide.Terminal} Console"))
        {
            ImGui.End();
            return;
        }

        // Options bar
        ImGui.Checkbox("Auto scroll", ref _autoScroll);
        ImGui.SameLine();
        if (ImGui.SmallButton($"{Lucide.Trash} Clear"))
            _writer.lines.ToList().Clear(); // or expose a Clear() on the writer

        ImGui.Separator();

        // Scrollable log region
        ImGui.BeginChild("ConsoleScrollRegion", new Vector2(0, 0), ImGuiChildFlags.None);

        foreach (var line in _writer.lines)
        {
            // Color code lines based on prefix
            if (line.StartsWith("[ERR]"))
                ImGui.TextColored(new Vector4(1f, 0.3f, 0.3f, 1f), line);
            else if (line.StartsWith("[WARN]"))
                ImGui.TextColored(new Vector4(1f, 0.8f, 0.0f, 1f), line);
            else if (line.StartsWith("[INFO]"))
                ImGui.TextColored(new Vector4(0.3f, 1f, 0.5f, 1f), line);
            else
                ImGui.TextUnformatted(line);
        }

        if (_autoScroll && _writer.ScrollToBottom)
        {
            ImGui.SetScrollHereY(1.0f);
            _writer.ScrollToBottom = false; // reset flag via a method
        }

        ImGui.EndChild();
        ImGui.End();
    }
}