using System.Text;

namespace ArcadianEngine.Utils;

public class ImGuiConsoleWriter(int maxLines = 200) : TextWriter
{
    private readonly List<string> _lines = [];
    private string _currentLine = "";
    private readonly int _maxLines = maxLines;

    public IReadOnlyList<string> lines => _lines;
    public bool ScrollToBottom;

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        if (value == '\n')
        {
            _lines.Add(_currentLine);
            if (_lines.Count > _maxLines)
                _lines.RemoveAt(0);
            _currentLine = "";
            ScrollToBottom = true;
        }
        else
        {
            _currentLine += value;
        }
    }

    public override void Write(string? value)
    {
        if (value == null) return;
        foreach (var c in value)
            Write(c);
    }
}