using Friflo.Engine.ECS;
using System.Numerics;

namespace ArcadianEngine.Components;

public struct Transform2D(Vector2 position, Vector2 scale, float rotation) : IComponent
{
    public Vector2 position = position;
    public Vector2 scale = scale;
    public float rotation = rotation;

    public Transform2D() : this(Vector2.Zero, Vector2.One, 0.0f) { }

    public Transform2D(Vector2 position) : this(position, Vector2.One, 0.0f) { }

    // Direction the entity is facing based on rotation
    public readonly Vector2 Forward => new(MathF.Cos(rotation), MathF.Sin(rotation));
    public readonly Vector2 Right => new(MathF.Sin(rotation), -MathF.Cos(rotation));
}
