using ArcadianEngine.Math;

namespace ArcadianEngine.Editor;

public partial class Architect<TG>(TG game, string title, Vector2I windowSize) : Game<TG>(game, title, windowSize)
    where TG : class, IArcadianGame<TG>
{
    protected override void Initialize()
    {
        Console.WriteLine("Test");
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }

    protected override void Draw(float deltaTime)
    {
        base.Draw(deltaTime);
    }
}