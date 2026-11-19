namespace ScheduleApp.Models;

public class Room
{
    private readonly string number;
    private readonly List<Session> sessions = new();

    public Room(string number)
    {
        this.number = number;
    }

    public string Number => number;

    public static Room FromAttributes(IReadOnlyDictionary<string, string> attributes)
    {
        return new Room(attributes.TryGetValue("number", out var number) ? number : "");
    }

    public List<Session> GetSessions() => sessions;
}
