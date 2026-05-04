using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record class DrawRectCommand(int Layer, Rectangle Rect, Color Color) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawRectangleRec(Rect, Color);
}
