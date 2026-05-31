namespace ArcadianEngine.Editor;

public interface IArcadianArchitect<TSelf> : IArcadianGame<TSelf> where TSelf : class, IArcadianGame<TSelf>
{
}