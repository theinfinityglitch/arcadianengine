using System.Numerics;
using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record class DrawRectangleCommand(int Layer, Rectangle Rect, Vector2 Origin, float Rotation, Color Color) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawRectanglePro(Rect, Origin, Rotation, Color);
}
