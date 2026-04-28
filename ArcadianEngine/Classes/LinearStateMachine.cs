namespace ArcadianEngine.Classes;

public class LinearStateMachine(string stateMachineName) : StateMachine(stateMachineName)
{
    protected string _defaultStateName = "";
    protected string _currentStateName = "";

    public override void AddState(string stateName, State state)
    {
        if (_states.Count == 0)
        {
            _defaultStateName = stateName;
            _currentStateName = stateName;
        }

        base.AddState(stateName, state);
    }

    public virtual void Initialize()
    {
        ChangeState(_defaultStateName);
    }

    public void ChangeState(string stateName)
    {
        if (stateName == _currentStateName || stateName == "" || !_states.ContainsKey(stateName))
        {
            return;
        }

        Console.WriteLine($"Set state {_stateMachineName}::{stateName}");

        _states[_currentStateName].OnExit();
        _currentStateName = stateName;
        _states[_currentStateName].OnEnter();
    }

    public virtual int HandleInput()
    {
        if (_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _states[_currentStateName].OnHandleInput();

        return 0;
    }

    public virtual int Update(float deltaTime)
    {
        if (_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _states[_currentStateName].OnUpdate(deltaTime);

        return 0;
    }

    public virtual int Draw()
    {
        if (_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _states[_currentStateName].OnDraw();

        return 0;
    }
}
