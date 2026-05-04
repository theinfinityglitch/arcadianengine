using System.Numerics;
using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record class DrawRingCommand(int Layer, Vector2 Center, float InnerRadius, float OuterRadius, float StartAngle, float EndAngle, int Segments, Color Color) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawRing(Center, InnerRadius, OuterRadius, StartAngle, EndAngle, Segments, Color);
}
