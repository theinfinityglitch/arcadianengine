using System.Numerics;
using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record class DrawRectangleCommand(int Layer, Rectangle rect, Vector2 origin, float rotation, Color color) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawRectanglePro(rect, origin, rotation, color);
}
