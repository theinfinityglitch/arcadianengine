using System.Numerics;

namespace ArcadianEngine.Math;

public struct Vector2I(int x, int y) : IEquatable<Vector2I>
{
    public int X = x;
    public int Y = y;

    // --- Compatibility with System.Numerics.Vector2 ---
    public static explicit operator Vector2(Vector2I value) => new(value.X, value.Y);
    public static explicit operator Vector2I(Vector2 value) => new((int)value.X, (int)value.Y);

    // --- Basic Operators ---
    public static Vector2I operator +(Vector2I left, Vector2I right) => new(left.X + right.X, left.Y + right.Y);
    public static Vector2I operator -(Vector2I left, Vector2I right) => new(left.X - right.X, left.Y - right.Y);
    public static Vector2I operator *(Vector2I left, Vector2I right) => new(left.X * right.X, left.Y * right.Y);
    public static Vector2I operator *(Vector2I value, int scale) => new(value.X * scale, value.Y * scale);

    // --- Equality ---
    public readonly bool Equals(Vector2I other) => X == other.X && Y == other.Y;
    public override readonly bool Equals(object? obj) => obj is Vector2I other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(X, Y);
    public static bool operator ==(Vector2I left, Vector2I right) => left.Equals(right);
    public static bool operator !=(Vector2I left, Vector2I right) => !left.Equals(right);

    public override readonly string ToString() => $"<{X}, {Y}>";
}
