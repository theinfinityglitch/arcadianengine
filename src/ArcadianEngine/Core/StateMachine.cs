using ArcadianEngine.Exceptions;

namespace ArcadianEngine.Core;

public class StateMachine<TG>(string stateMachineName, GameContext<TG> cx) where TG : class, IArcadianGame<TG>
{
    public readonly string StateMachineName = stateMachineName;

    protected Dictionary<string, State<TG>> States = [];
    protected GameContext<TG> Context = cx;
    protected bool Initialized;

    public virtual void AddState<T>(T state) where T : State<TG>
    {
        var stateName = typeof(T).Name;
        state.Context = Context;
        state.SetOwnerStateMachine(this);
        States.Add(stateName, state);
    }

    public virtual void Initialize() { Initialized = true; }

    public virtual void HandleInput()
    {
        if (States.Count == 0) throw new EmptyStateMachineException(StateMachineName);
        if (!Initialized) throw new StateMachineNotInitializedException(StateMachineName);
    }

    public virtual void Update(float deltaTime)
    {
        if (States.Count == 0) throw new EmptyStateMachineException(StateMachineName);
        if (!Initialized) throw new StateMachineNotInitializedException(StateMachineName);
    }

    public virtual void Draw()
    {
        if (States.Count == 0) throw new EmptyStateMachineException(StateMachineName);
        if (!Initialized) throw new StateMachineNotInitializedException(StateMachineName);
    }
}
