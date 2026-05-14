namespace ArcadianEngine.Exceptions;

public class EmptyStateMachineException(string stateMachineName) : Exception($"No states found for {stateMachineName}.") { }

public class StateMachineNotInitializedException(string stateMachineName) : Exception($"{stateMachineName} used before Initialization!") { }
