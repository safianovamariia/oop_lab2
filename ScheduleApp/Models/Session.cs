namespace ScheduleApp.Models;

public abstract class Session
{
    protected readonly string day;
    protected readonly string time;
    protected readonly Room room;

    protected Session(Room room, string day, string time)
    {
        this.room = room;
        this.day = day;
        this.time = time;
        room.GetSessions().Add(this);
    }

    public static Session Create(Room room, IReadOnlyDictionary<string, string> attributes)
    {
        string Get(string name) => attributes.TryGetValue(name, out var value) ? value : "";

        if (attributes.ContainsKey("subject"))
            return new Lesson(room, Get("day"), Get("time"), Get("subject"), Get("teacher"), Get("group"));
        if (attributes.ContainsKey("workplace"))
            return new ArmWork(room, Get("day"), Get("time"), Get("workplace"), Get("user"));
        return new FreeAccess(room, Get("day"), Get("time"), Get("responsible"));
    }

    public virtual string GetAttribute(string name)
    {
        return name switch
        {
            "room" => room.Number,
            "day" => day,
            _ => ""
        };
    }

    public abstract string GetInfo();

    protected string Header() => $"Room {room.Number} | {day} {time}";
}
