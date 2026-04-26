namespace ArcadianEngine.Classes;

public interface ISchedule
{
    public string GetLabel()
    {
        return GetType().Name;
    }
}
