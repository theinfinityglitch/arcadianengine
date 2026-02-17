namespace ArcadianEngine.Classes;

public struct Vector2i(int x, int y)
{
    public int x = x, y = y;

    public static Vector2i Zero()
    {
        return new(0, 0);
    }

    public static Vector2i Left()
    {
        return new(-1, 0);
    }

    public static Vector2i Right()
    {
        return new(1, 0);
    }

    public static Vector2i Up()
    {
        return new(0, -1);
    }

    public static Vector2i Down()
    {
        return new(0, 1);
    }
}
