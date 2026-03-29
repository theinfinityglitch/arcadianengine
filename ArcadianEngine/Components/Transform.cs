using Friflo.Engine.ECS;
using System.Numerics;

namespace ArcadianEngine.Components;

public struct Transform2D(Vector2 position, Vector2 scale, float rotation) : IComponent
{
    public Vector2 position = position;
    public Vector2 scale = scale;
    public float rotation = rotation;

    public Transform2D(Vector2 position) : this(position, Vector2.One, 0.0f) { }
}
