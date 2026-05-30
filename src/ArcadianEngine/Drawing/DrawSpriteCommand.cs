using System.Numerics;
using Raylib_cs;

namespace ArcadianEngine.Drawing;

public record DrawSpriteCommand(int Layer, Texture2D texture, Vector2 position, Color tint) : DrawCommand(Layer)
{
    public override void Execute()
        => Raylib.DrawTextureV(texture, position, tint);
}
