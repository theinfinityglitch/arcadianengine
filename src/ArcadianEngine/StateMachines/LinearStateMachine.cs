using ArcadianEngine.Core;
using ArcadianEngine.Exceptions;

namespace ArcadianEngine.StateMachines;

public sealed class LinearStateMachine<TG>(string stateMachineName, GameContext<TG> cx) : StateMachine<TG>(stateMachineName, cx) where TG : class, IArcadianGame<TG>
{
    private string DefaultStateName = "";
    private string CurrentStateName = "";

    public override void AddState<T>(T state)
    {
        if (States.Count == 0)
        {
            DefaultStateName = typeof(T).Name;
        }

        base.AddState(state);
    }

    public override void Initialize()
    {
        if (DefaultStateName != "") ChangeState(DefaultStateName);

        base.Initialize();
    }

    public void ChangeState(string stateName)
    {
        if (!States.ContainsKey(stateName) || stateName == "") throw new StateNotFoundException(StateMachineName, stateName);
        if (stateName == CurrentStateName) throw new EmptyStateNameException(StateMachineName);

        Console.WriteLine($"Set state {StateMachineName}::{stateName}");

        if (CurrentStateName != "") States[CurrentStateName].OnExit();
        CurrentStateName = stateName;
        States[CurrentStateName].OnEnter();
    }

    public override void HandleInput()
    {
        if (CurrentStateName == "") return;

        base.HandleInput();

        States[CurrentStateName].OnHandleInput();
    }

    public override void Update(float deltaTime)
    {
        if (CurrentStateName == "") return;

        base.Update(deltaTime);

        States[CurrentStateName].OnUpdate(deltaTime);
    }

    public override void Draw()
    {
        if (CurrentStateName == "") return;

        base.Draw();

        States[CurrentStateName].OnDraw();
    }
}
