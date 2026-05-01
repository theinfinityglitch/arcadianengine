namespace ArcadianEngine.Core;

public class State
{
    protected StateMachine? _ownerStateMachine = null;

    public virtual void OnEnter() { }

    public virtual void OnOwnerSet() { }

    public virtual void OnHandleInput() { }

    public virtual void OnUpdate(float deltaTime) { }

    public virtual void OnDraw() { }

    public virtual void OnExit() { }

    public void SetOwnerStateMachine(StateMachine owner)
    {
        this._ownerStateMachine = owner;
        OnOwnerSet();
    }
}
