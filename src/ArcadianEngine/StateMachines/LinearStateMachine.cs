using ArcadianEngine.Core;
using ArcadianEngine.Exceptions;

namespace ArcadianEngine.StateMachines;

public class LinearStateMachine<G>(string stateMachineName, GameContext<G> cx) : StateMachine<G>(stateMachineName, cx) where G : class, IArcadianGame<G>
{
    protected string _defaultStateName = "";
    protected string _currentStateName = "";

    public override void AddState<T>(T state)
    {
        if (_states.Count == 0)
        {
            _defaultStateName = typeof(T).Name;
        }

        base.AddState(state);
    }

    public override void Initialize()
    {
        if (_defaultStateName != "") ChangeState(_defaultStateName);

        base.Initialize();
    }

    public virtual void ChangeState(string stateName)
    {
        if (!_states.ContainsKey(stateName) || stateName == "") throw new StateNotFoundException(stateMachineName, stateName);
        if (stateName == _currentStateName) throw new EmptyStateNameException(stateMachineName);

        Console.WriteLine($"Set state {stateMachineName}::{stateName}");

        if (_currentStateName != "") _states[_currentStateName].OnExit();
        _currentStateName = stateName;
        _states[_currentStateName].OnEnter();
    }

    public override void HandleInput()
    {
        if (_currentStateName == "") return;

        base.HandleInput();

        _states[_currentStateName].OnHandleInput();
    }

    public override void Update(float deltaTime)
    {
        if (_currentStateName == "") return;

        base.Update(deltaTime);

        _states[_currentStateName].OnUpdate(deltaTime);
    }

    public override void Draw()
    {
        if (_currentStateName == "") return;

        base.Draw();

        _states[_currentStateName].OnDraw();
    }
}
