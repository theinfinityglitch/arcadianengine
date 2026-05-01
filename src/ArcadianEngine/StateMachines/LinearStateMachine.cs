using ArcadianEngine.Core;

namespace ArcadianEngine.StateMachines;

public class LinearStateMachine<G>(string stateMachineName, GameContext<G> cx) : StateMachine<G>(stateMachineName, cx) where G : class, IArcadianGame<G>
{
    protected string _defaultStateName = "";
    protected string _currentStateName = "";

    public virtual void AddState<T>(T state) where T : State<G>
    {
        string stateName = typeof(T).Name;

        if (_states.Count == 0)
        {
            _defaultStateName = stateName;
        }

        _states.Add(stateName, state);
        state.SetOwnerStateMachine(this, context);
    }

    public virtual void Initialize()
    {
        ChangeState(_defaultStateName);
    }

    public virtual void ChangeState(string stateName)
    {
        if (stateName == _currentStateName || stateName == "" || !_states.ContainsKey(stateName))
        {
            return;
        }

        Console.WriteLine($"Set state {stateMachineName}::{stateName}");

        if (_currentStateName != "")
            _states[_currentStateName].OnExit(context);
        _currentStateName = stateName;
        _states[_currentStateName].OnEnter(context);
    }

    public virtual int HandleInput()
    {
        if (_states.Count == 0)
        {
            Console.WriteLine($"No states found for {stateMachineName}.");
            return 1;
        }

        _states[_currentStateName].OnHandleInput(context);

        return 0;
    }

    public virtual int Update(float deltaTime)
    {
        if (_states.Count == 0)
        {
            Console.WriteLine($"No states found for {stateMachineName}.");
            return 1;
        }

        _states[_currentStateName].OnUpdate(deltaTime, context);

        return 0;
    }

    public virtual int Draw()
    {
        if (_states.Count == 0)
        {
            Console.WriteLine($"No states found for {stateMachineName}.");
            return 1;
        }

        _states[_currentStateName].OnDraw(context);

        return 0;
    }
}
