namespace ScheduleApp.Models;

public class ArmWork : Session
{
    private readonly string workplace;
    private readonly string user;

    public ArmWork(Room room, string day, string time, string workplace, string user)
        : base(room, day, time)
    {
        this.workplace = workplace;
        this.user = user;
    }

    public override string GetAttribute(string name)
    {
        return name switch
        {
            "teacher" => user,
            _ => base.GetAttribute(name)
        };
    }

    public override string GetInfo() =>
        $"{Header()} | ARM work: workplace {workplace}, user {user}";
}
