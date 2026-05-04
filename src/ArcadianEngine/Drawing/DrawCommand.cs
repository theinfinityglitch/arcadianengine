namespace ArcadianEngine.Drawing;

public abstract record DrawCommand(int Layer)
{
    public abstract void Execute();
}
