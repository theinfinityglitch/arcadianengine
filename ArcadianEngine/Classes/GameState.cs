namespace ArcadianEngine.Classes;

public class GameState : State
{
    public virtual void Enter(Game cx) { }

    public virtual void OnOwnerSet(Game cx) { }

    public virtual void HandleInput(Game cx) { }

    public virtual void Update(Game cx, float deltaTime) { }

    public virtual void Draw(Game cx) { }

    public virtual void Exit(Game cx) { }
}
