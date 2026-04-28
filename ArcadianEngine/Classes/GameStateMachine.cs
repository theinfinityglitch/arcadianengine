namespace ArcadianEngine.Classes;

public class GameStateMachine(string stateMachineName) : StateMachine(stateMachineName)
{
    protected Dictionary<string, GameState> _game_states = [];
    protected string _defaultStateName = "";
    protected string _currentStateName = "";

    public virtual void AddState(Game cx, string stateName, GameState state)
    {
        if (_game_states.Count == 0)
        {
            _defaultStateName = stateName;
        }

        _game_states.Add(stateName, state);
        state.SetOwnerStateMachine(cx, this);
    }

    public virtual void Initialize(Game cx)
    {
        ChangeState(_defaultStateName, cx);
    }

    public virtual void ChangeState(string stateName, Game cx)
    {
        if (stateName == _currentStateName || stateName == "" || !_game_states.ContainsKey(stateName))
        {
            return;
        }

        Console.WriteLine($"Set state {_stateMachineName}::{stateName}");

        if (_currentStateName != "")
            _game_states[_currentStateName].OnExit(cx);
        _currentStateName = stateName;
        _game_states[_currentStateName].OnEnter(cx);
    }

    public virtual int HandleInput(Game cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].OnHandleInput(cx);

        return 0;
    }

    public virtual int Update(float deltaTime, Game cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].OnUpdate(cx, deltaTime);

        return 0;
    }

    public virtual int Draw(Game cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].OnDraw(cx);

        return 0;
    }
}
