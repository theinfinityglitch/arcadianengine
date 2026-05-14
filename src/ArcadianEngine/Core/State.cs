namespace ArcadianEngine.Core;

public class State<G> where G : class, IArcadianGame<G>
{
    protected StateMachine<G>? _ownerStateMachine = null;

#pragma warning disable CS8618
    public GameContext<G> Context;
#pragma warning restore CS8618

    public virtual void OnEnter() { }

    public virtual void OnOwnerSet() { }

    public virtual void OnHandleInput() { }

    public virtual void OnUpdate(float deltaTime) { }

    public virtual void OnDraw() { }

    public virtual void OnExit() { }

    public void SetOwnerStateMachine(StateMachine<G> owner)
    {
        this._ownerStateMachine = owner;
        OnOwnerSet();
    }
}
