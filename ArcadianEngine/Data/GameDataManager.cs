using ArcadianEngine.Utils.Serialization;

namespace ArcadianEngine.Data;

public class GameDataManager(IArcadianGame? game) : IDisposable
{
    private readonly IArcadianGame? _game = game;
    string? binResourcesDirectory;
    private const string GameProfileFileName = @"game_profile";
    protected GameProfile? _gameProfile = null;

    public void Initialize(string resourcesBinPath = "resources")
    {
        binResourcesDirectory = resourcesBinPath;
        LoadGameSettings();
    }

    public void LoadGameSettings()
    {
        string gameProfilePath = FileHelper.GetPath(Path.Join(binResourcesDirectory, GameProfileFileName));

        if (_gameProfile is null && File.Exists(gameProfilePath))
        {
            Console.WriteLine(File.ReadAllText(gameProfilePath));
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
