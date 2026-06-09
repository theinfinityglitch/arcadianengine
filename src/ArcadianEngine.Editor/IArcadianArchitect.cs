namespace ArcadianEngine.Editor;

public interface IArcadianArchitect<TSelf, TGame> : IArcadianGame<TGame>
    where TSelf : class, IArcadianArchitect<TSelf, TGame> where TGame : class, IArcadianGame<TGame>
{
}