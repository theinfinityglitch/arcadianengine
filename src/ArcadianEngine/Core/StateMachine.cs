namespace ArcadianEngine.Core;

public class StateMachine<G>(string stateMachineName, GameContext<G> cx) where G : class, IArcadianGame<G>
{
    public readonly string stateMachineName = stateMachineName;

    protected Dictionary<string, State<G>> _states = [];
    protected GameContext<G> context = cx;

    public virtual void AddState(string stateName, State<G> state)
    {
        _states.Add(stateName, state);
        state.SetOwnerStateMachine(this, context);
    }
}
