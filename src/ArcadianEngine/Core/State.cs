namespace ArcadianEngine.Core;

public class State<TG> where TG : class, IArcadianGame<TG>
{
    protected StateMachine<TG>? OwnerStateMachine;

#pragma warning disable CS8618
    public GameContext<TG> Context;
#pragma warning restore CS8618

    public virtual void OnEnter() { }

    public virtual void OnOwnerSet() { }

    public virtual void OnHandleInput() { }

    public virtual void OnUpdate(float deltaTime) { }

    public virtual void OnDraw() { }

    public virtual void OnExit() { }

    public void SetOwnerStateMachine(StateMachine<TG> owner)
    {
        OwnerStateMachine = owner;
        OnOwnerSet();
    }
}
