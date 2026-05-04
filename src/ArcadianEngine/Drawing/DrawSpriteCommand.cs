using System.Numerics;

using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record DrawSpriteCommand(int Layer, Texture2D Texture, Vector2 Position, Color Tint) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawTextureV(Texture, Position, Tint);
}
