using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record class DrawRectCommand(int Layer, Rectangle rect, Color color) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawRectangleRec(rect, color);
}
