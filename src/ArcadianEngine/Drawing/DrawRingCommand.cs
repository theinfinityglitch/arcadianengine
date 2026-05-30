using System.Numerics;
using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record class DrawRingCommand(int Layer, Vector2 center, float innerRadius, float outerRadius, float startAngle, float endAngle, int segments, Color color) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawRing(center, innerRadius, outerRadius, startAngle, endAngle, segments, color);
}
