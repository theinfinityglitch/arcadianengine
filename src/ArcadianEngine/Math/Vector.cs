using System.Numerics;

namespace ArcadianEngine.Math;

public struct Vector2i(int x, int y) : IEquatable<Vector2i>
{
    public int X = x;
    public int Y = y;

    // --- Compatibility with System.Numerics.Vector2 ---
    public static explicit operator Vector2(Vector2i value) => new(value.X, value.Y);
    public static explicit operator Vector2i(Vector2 value) => new((int)value.X, (int)value.Y);

    // --- Basic Operators ---
    public static Vector2i operator +(Vector2i left, Vector2i right) => new(left.X + right.X, left.Y + right.Y);
    public static Vector2i operator -(Vector2i left, Vector2i right) => new(left.X - right.X, left.Y - right.Y);
    public static Vector2i operator *(Vector2i left, Vector2i right) => new(left.X * right.X, left.Y * right.Y);
    public static Vector2i operator *(Vector2i value, int scale) => new(value.X * scale, value.Y * scale);

    // --- Equality ---
    public readonly bool Equals(Vector2i other) => X == other.X && Y == other.Y;
    public override readonly bool Equals(object? obj) => obj is Vector2i other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(X, Y);
    public static bool operator ==(Vector2i left, Vector2i right) => left.Equals(right);
    public static bool operator !=(Vector2i left, Vector2i right) => !left.Equals(right);

    public override readonly string ToString() => $"<{X}, {Y}>";
}
