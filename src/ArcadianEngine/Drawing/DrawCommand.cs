namespace ArcadianEngine.Drawing;

public abstract record DrawCommand(int layer)
{
    public abstract void Execute();
}
