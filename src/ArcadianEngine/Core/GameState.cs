using ArcadianEngine.StateMachines;

namespace ArcadianEngine.Core;

public class GameState<G> where G : class, IArcadianGame<G>
{
    protected GameStateMachine<G>? _ownerStateMachine = null;

    public virtual void OnEnter(GameContext<G> cx) { }

    public virtual void OnOwnerSet(GameContext<G> cx) { }

    public virtual void OnHandleInput(GameContext<G> cx) { }

    public virtual void OnUpdate(float deltaTime, GameContext<G> cx) { }

    public virtual void OnDraw(GameContext<G> cx) { }

    public virtual void OnExit(GameContext<G> cx) { }

    public void SetOwnerStateMachine(GameStateMachine<G> owner, GameContext<G> cx)
    {
        this._ownerStateMachine = owner;
        OnOwnerSet(cx);
    }
}
