namespace ArcadianEngine.Classes;

public class GameStateMachine(string stateMachineName) : LinearStateMachine(stateMachineName)
{
    protected Dictionary<string, GameState> _game_states = [];

    public virtual void AddState(string stateName, GameState state)
    {
        if (_game_states.Count == 0)
        {
            _defaultStateName = stateName;
        }

        _game_states.Add(stateName, state);
        state.SetOwnerStateMachine(this);
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
            _game_states[_currentStateName].Exit(cx);
        _currentStateName = stateName;
        _game_states[_currentStateName].Enter(cx);
    }

    public virtual int HandleInput(Game cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].HandleInput(cx);

        return 0;
    }

    public virtual int Update(float deltaTime, Game cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].Update(cx, deltaTime);

        return 0;
    }

    public virtual int Draw(Game cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].Draw(cx);

        return 0;
    }
}
