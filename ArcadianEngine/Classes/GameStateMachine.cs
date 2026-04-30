namespace ArcadianEngine.Classes;

public class GameStateMachine<G> : StateMachine where G : class, IArcadianGame<G>
{
    protected Dictionary<string, GameState<G>> _game_states = [];
    protected string _defaultStateName = "";
    protected string _currentStateName = "";

    public GameStateMachine() : base("GameStateMachine") { }

    public virtual void AddState(string stateName, GameState<G> state, GameContext<G> cx)
    {
        if (_game_states.Count == 0)
        {
            _defaultStateName = stateName;
        }

        _game_states.Add(stateName, state);
        state.SetOwnerStateMachine(this, cx);
    }

    public virtual void Initialize(GameContext<G> cx)
    {
        ChangeState(_defaultStateName, cx);
    }

    public virtual void ChangeState(string stateName, GameContext<G> cx)
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

    public virtual int HandleInput(GameContext<G> cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].OnHandleInput(cx);

        return 0;
    }

    public virtual int Update(float deltaTime, GameContext<G> cx)
    {
        if (_game_states.Count == 0)
        {
            Console.WriteLine($"No states found for {_stateMachineName}.");
            return 1;
        }

        _game_states[_currentStateName].OnUpdate(deltaTime, cx);

        return 0;
    }

    public virtual int Draw(GameContext<G> cx)
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
