namespace ArcadianEngine.Classes;

public class StateMachine(string stateMachineName)
{
    protected string _stateMachineName = stateMachineName;
    protected Dictionary<string, State> _states = [];

    public virtual void AddState(string stateName, State state)
    {
        _states.Add(stateName, state);
        state.SetOwnerStateMachine(this);
    }
}
