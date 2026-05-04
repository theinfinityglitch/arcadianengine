using System.Numerics;

using Raylib_cs;

using ArcadianEngine.Drawing;
using ArcadianEngine.Math;

namespace ArcadianEngine.Resources;

public class RenderPipeline(Vector2i virtualSize)
{
    private readonly SortedDictionary<int, List<DrawCommand>> _commands = [];
    private readonly Dictionary<int, RenderTexture2D> _layerTextures = [];

    public Vector2i virtualSize = virtualSize;

    public void Draw(DrawCommand command)
    {
        if (!_commands.ContainsKey(command.Layer))
            _commands[command.Layer] = [];
        _commands[command.Layer].Add(command);
    }

    public void DrawSprite(Texture2D tex, Vector2 pos, int layer = 0, Color? tint = null)
        => Draw(new DrawSpriteCommand(layer, tex, pos, tint ?? Color.White));

    public void DrawRect(Rectangle rect, Color color, int layer = 0)
        => Draw(new DrawRectCommand(layer, rect, color));

    public void DrawRectangle(Rectangle rect, Vector2 origin, float rotation, Color color, int layer = 0)
        => Draw(new DrawRectangleCommand(layer, rect, origin, rotation, color));

    public void DrawRing(Vector2 center, float innerRadius, float outerRadius, float startAngle, float endAngle, int segments, Color color, int layer)
        => Draw(new DrawRingCommand(layer, center, innerRadius, outerRadius, startAngle, endAngle, segments, color));

    protected RenderTexture2D GetOrCreateLayerTexture(int layer)
    {
        if (_layerTextures.TryGetValue(layer, out var texture))
            return texture;

        var newTexture = Raylib.LoadRenderTexture(virtualSize.x, virtualSize.y);
        _layerTextures[layer] = newTexture;
        return newTexture;
    }

    public RenderTexture2D Flush()
    {
        // Final composed texture
        var output = Raylib.LoadRenderTexture(virtualSize.x, virtualSize.y);

        Raylib.BeginTextureMode(output);
        Raylib.ClearBackground(Color.Black);
        Raylib.EndTextureMode();

        foreach (var (layer, commands) in _commands)
        {
            var layerTex = GetOrCreateLayerTexture(layer);

            Raylib.BeginTextureMode(layerTex);
            Raylib.ClearBackground(Color.Blank);

            foreach (var cmd in commands)
                cmd.Execute();

            Raylib.EndTextureMode();

            // // Compose layer onto output
            // // (apply per-layer shader here if needed)
            Raylib.BeginTextureMode(output);
            Raylib.DrawTextureRec(
                layerTex.Texture,
                new Rectangle(0, 0, virtualSize.x, -virtualSize.y), // flip Y
                Vector2.Zero,
                Color.White
            );
            Raylib.EndTextureMode();
        }

        _commands.Clear();

        return output;
    }

    public void PresentToScreen(RenderTexture2D frame)
    {
        int screenW = Raylib.GetRenderWidth();
        int screenH = Raylib.GetRenderHeight();

        float scale = System.Math.Min(
            (float)screenW / virtualSize.x,
            (float)screenH / virtualSize.y
        );

        int destW = (int)(virtualSize.x * scale);
        int destH = (int)(virtualSize.y * scale);
        int offsetX = (screenW - destW) / 2;
        int offsetY = (screenH - destH) / 2;

        Raylib.ClearBackground(Color.Black); // letterbox color

        Raylib.DrawTexturePro(
            frame.Texture,
            new Rectangle(0, 0, virtualSize.x, -virtualSize.y), // flip Y
            new Rectangle(offsetX, offsetY, destW, destH),
            Vector2.Zero,
            0f,
            Color.White
        );
    }

    public void Dispose()
    {
        foreach (var tex in _layerTextures.Values)
            Raylib.UnloadRenderTexture(tex);
        _layerTextures.Clear();
    }
}
