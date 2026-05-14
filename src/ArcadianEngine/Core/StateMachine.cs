using ArcadianEngine.Exceptions;

namespace ArcadianEngine.Core;

public class StateMachine<G>(string stateMachineName, GameContext<G> cx) where G : class, IArcadianGame<G>
{
    public readonly string stateMachineName = stateMachineName;

    protected Dictionary<string, State<G>> _states = [];
    protected GameContext<G> Context = cx;
    protected bool Initialized = false;

    public virtual void AddState<T>(T state) where T : State<G>
    {
        string stateName = typeof(T).Name;
        state.Context = Context;
        state.SetOwnerStateMachine(this);
        _states.Add(stateName, state);
    }

    public virtual void Initialize() { Initialized = true; }

    public virtual void HandleInput()
    {
        if (_states.Count == 0) throw new EmptyStateMachineException(stateMachineName);
        if (!Initialized) throw new StateMachineNotInitializedException(stateMachineName);
    }

    public virtual void Update(float deltaTime)
    {
        if (_states.Count == 0) throw new EmptyStateMachineException(stateMachineName);
        if (!Initialized) throw new StateMachineNotInitializedException(stateMachineName);
    }

    public virtual void Draw()
    {
        if (_states.Count == 0) throw new EmptyStateMachineException(stateMachineName);
        if (!Initialized) throw new StateMachineNotInitializedException(stateMachineName);
    }
}
