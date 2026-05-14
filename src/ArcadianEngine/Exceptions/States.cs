namespace ArcadianEngine.Exceptions;

public class StateNotFoundException(string stateMachineName, string stateName) : Exception($"State {stateName} was not found for {stateMachineName}.") { }

public class EmptyStateNameException(string stateMachineName) : Exception($"{stateMachineName}: Trying to use a empty named state is not allowed!") { }
