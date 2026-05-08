using Friflo.Engine.ECS;
using System.Numerics;

namespace ArcadianEngine.Components;

public struct Transform2D(Vector2 position, Vector2 scale, float rotation, int zIndex = 0) : IComponent
{
    public Vector2 Position = position;
    public Vector2 Scale = scale;
    public float Rotation = rotation;
    public int ZIndex = zIndex;

    public Transform2D() : this(Vector2.Zero, Vector2.One, 0f, 0) { }
    public Transform2D(Vector2 position) : this(position, Vector2.One, 0f, 0) { }

    public readonly Vector2 Forward => new(MathF.Cos(Rotation), MathF.Sin(Rotation));
    public readonly Vector2 Right => new(MathF.Sin(Rotation), -MathF.Cos(Rotation));
}

public struct GlobalTransform2D : IComponent
{
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;
    public int ZIndex;

    // Computed from Transform2D + parent's GlobalTransform2D
    public readonly Vector2 Forward => new(MathF.Cos(Rotation), MathF.Sin(Rotation));
    public readonly Vector2 Right => new(MathF.Sin(Rotation), -MathF.Cos(Rotation));
}
