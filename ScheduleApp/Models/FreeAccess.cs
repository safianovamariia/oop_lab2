namespace ScheduleApp.Models;

public class FreeAccess : Session
{
    private readonly string responsible;

    public FreeAccess(Room room, string day, string time, string responsible)
        : base(room, day, time)
    {
        this.responsible = responsible;
    }

    public override string GetInfo() =>
        $"{Header()} | Free access, responsible: {responsible}";
}
