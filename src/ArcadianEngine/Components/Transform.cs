using System.Numerics;
using Friflo.Engine.ECS;

namespace ArcadianEngine.Components;

public struct Transform2D(Vector2 position, Vector2 scale, float rotation, int zIndex = 0) : IComponent
{
    [Export] public Vector2 Position = position;
    [Export] public Vector2 Scale = scale;
    [Export] public float Rotation = rotation;
    [Export] public int ZIndex = zIndex;

    public Vector2 GlobalPosition = position;
    public Vector2 GlobalScale = scale;
    public float GlobalRotation = rotation;
    public int GlobalZIndex = zIndex;

    public Transform2D() : this(Vector2.Zero, Vector2.One, 0f) { }
    public Transform2D(Vector2 position) : this(position, Vector2.One, 0f) { }
    public Transform2D(int zIndex) : this(Vector2.Zero, Vector2.One, 0f, zIndex) { }

    public readonly Vector2 forward => new(MathF.Cos(Rotation), MathF.Sin(Rotation));
    public readonly Vector2 backward => new(-MathF.Cos(Rotation), -MathF.Sin(Rotation));
    public readonly Vector2 right => new(MathF.Sin(Rotation), -MathF.Cos(Rotation));
    public readonly Vector2 left => new(-MathF.Sin(Rotation), MathF.Cos(Rotation));
}
