namespace ArcadianEngine.Classes;

public class GameState
{
    protected GameStateMachine? _ownerStateMachine = null;

    public virtual void OnEnter(Game cx) { }

    public virtual void OnOwnerSet(Game cx) { }

    public virtual void OnHandleInput(Game cx) { }

    public virtual void OnUpdate(Game cx, float deltaTime) { }

    public virtual void OnDraw(Game cx) { }

    public virtual void OnExit(Game cx) { }

    public void SetOwnerStateMachine(Game cx, GameStateMachine owner)
    {
        this._ownerStateMachine = owner;
        OnOwnerSet(cx);
    }
}
